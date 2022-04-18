using Histograms;
using Histograms.Models;
using QueryOptimiser.Cost.Nodes;
using QueryOptimiser.Models;
using QueryParser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryOptimiser.Cost.EstimateCalculators
{
    public abstract class BaseEstimateCalculator : IEstimateCalculator
    {
        public IHistogramManager HistogramManager { get; set; }

        internal abstract INodeCost<JoinNode> JoinCost { get; set; }

        internal BaseEstimateCalculator(IHistogramManager manager)
        {
            HistogramManager = manager;
        }

        public IntermediateTable EstimateIntermediateTable(INode node, IntermediateTable intermediateTable)
        {
            if (node is JoinNode joinNode)
            {
                return EstimateJoinTable(joinNode, intermediateTable);
            }
            else
            {
                throw new ArgumentException("Non handled node type " + node.ToString());
            }
        }

        private IntermediateTable EstimateJoinTable(JoinNode node, IntermediateTable table)
        {
            if (node.Relation == null)
                throw new ArgumentException("node relation is not allowed to be null " + node.ToString());

            List<IntermediateBucket> buckets;
            List<Tuple<TableReferenceNode, string>> references = new List<Tuple<TableReferenceNode, string>>();

            buckets = GetBucketMatches(node.Relation, table, ref references);

            return new IntermediateTable(buckets, references);
        }
       
        #region Matches
        private List<IntermediateBucket> GetBucketMatches(JoinPredicateRelation relation, IntermediateTable table, ref List<Tuple<TableReferenceNode, string>> references)
        {
            if (relation.Type == RelationType.Type.Predicate && relation.LeafPredicate != null)
                return GetBucketMatches(relation.LeafPredicate, table, ref references);
            else if (relation.LeftRelation != null && relation.RightRelation != null)
            {
                List<Tuple<TableReferenceNode, string>> leftReferences = new List<Tuple<TableReferenceNode, string>>();
                List<Tuple<TableReferenceNode, string>> rightReferences = new List<Tuple<TableReferenceNode, string>>();
                List<IntermediateBucket> leftBuckets = GetBucketMatches(relation.LeftRelation, table, ref leftReferences);
                List<IntermediateBucket> rightBuckets = GetBucketMatches(relation.RightRelation, table, ref rightReferences);

                List<IntermediateBucket> overlap = new List<IntermediateBucket>();
                if (relation.Type == RelationType.Type.And)
                {
                    overlap = IntermediateBucket.MergeOnOverlap(leftBuckets, rightBuckets);
                    references = GetOverlappingReferences(leftReferences, rightReferences);
                }
                else if (relation.Type == RelationType.Type.Or)
                {
                    overlap = leftBuckets.Concat(rightBuckets).ToList();
                    references = leftReferences.Concat(rightReferences).ToList();
                }

                
                return overlap;
            }
            else
                throw new ArgumentException("Missing noderelation type " + relation.ToString());
        }

        private List<IntermediateBucket> GetBucketMatches(JoinPredicate predicate, IntermediateTable table, ref List<Tuple<TableReferenceNode, string>> references)
        {
            Tuple<List<IHistogramBucket>, List<IHistogramBucket>> bucketPair = GetBucketPair(predicate, table);
            Tuple<IHistogramBucket[], IHistogramBucket[]> bounds = GetBucketBounds(predicate, bucketPair.Item1, bucketPair.Item2);

            if (bounds.Item1.Length > 0 && bounds.Item2.Length > 0) {
                references.Add(new Tuple<TableReferenceNode, string>(predicate.LeftTable, predicate.LeftAttribute ));
                references.Add(new Tuple<TableReferenceNode, string>(predicate.RightTable, predicate.RightAttribute ));
            } else
                return new List<IntermediateBucket>();

            if (predicate.ComType == ComparisonType.Type.Equal)
                return GetEqualityMatches(predicate, bounds.Item1, bounds.Item2);
            else
                return GetInEqualityMatches(predicate, bounds.Item1, bounds.Item2);
        }

        private List<IntermediateBucket> GetEqualityMatches(JoinPredicate predicate, IHistogramBucket[] leftBuckets, IHistogramBucket[] rightBuckets)
        {
            List<IntermediateBucket> buckets = new List<IntermediateBucket>();
            int rightCutoff = 0;
            for (int i = 0; i < leftBuckets.Length; i++)
            {
                for (int j = rightCutoff; j < rightBuckets.Length; j++)
                {
                    if (DoesMatch(leftBuckets[i], rightBuckets[j]))
                    {
                        List<Tuple<TableReferenceNode, string, BucketEstimate>> information = new List<Tuple<TableReferenceNode, string, BucketEstimate>>();
                        information.Add(Tuple.Create(predicate.LeftTable, predicate.LeftAttribute, new BucketEstimate(leftBuckets[i], 
                            JoinCost.GetBucketEstimate(predicate.ComType, leftBuckets[i], rightBuckets[j]))));
                        information.Add(Tuple.Create(predicate.RightTable, predicate.RightAttribute, new BucketEstimate(rightBuckets[j], 
                            JoinCost.GetBucketEstimate(predicate.ComType, rightBuckets[j], leftBuckets[i]))));
                        buckets.Add(new IntermediateBucket(information));
                    }
                    else
                        rightCutoff = Math.Max(0, j - 1);
                }
            }
            return buckets;
        }

        private List<IntermediateBucket> GetInEqualityMatches(JoinPredicate predicate, IHistogramBucket[] leftBuckets, IHistogramBucket[] rightBuckets)
        {
            List<IntermediateBucket> buckets = new List<IntermediateBucket>();
            int rightCutoff = rightBuckets.Length - 1;
            for (int i = leftBuckets.Length - 1; i >= 0; i--)
            {
                for (int j = rightCutoff; j >= 0; j--)
                {
                    bool match = true;
                    switch (predicate.ComType)
                    {
                        case ComparisonType.Type.Less:
                            if (leftBuckets[i].ValueEnd.CompareTo(rightBuckets[j].ValueStart) < 0)
                                match = true;
                            else if (leftBuckets[i].ValueStart.CompareTo(rightBuckets[j].ValueEnd) < 0)
                                match = true;
                            else
                                match = false;
                            break;
                        case ComparisonType.Type.EqualOrLess:
                            if (leftBuckets[i].ValueEnd.CompareTo(rightBuckets[j].ValueStart) <= 0)
                                match = true;
                            else if (leftBuckets[i].ValueStart.CompareTo(rightBuckets[j].ValueEnd) <= 0)
                                match = true;
                            else
                                match = false;
                            break;
                        case ComparisonType.Type.More:
                            if (leftBuckets[i].ValueStart.CompareTo(rightBuckets[j].ValueEnd) > 0)
                                match = true;
                            else if (leftBuckets[i].ValueEnd.CompareTo(rightBuckets[j].ValueStart) > 0)
                                match = true;
                            else
                                match = false;
                            break;
                        case ComparisonType.Type.EqualOrMore:
                            if (leftBuckets[i].ValueStart.CompareTo(rightBuckets[j].ValueEnd) >= 0)
                                match = true;
                            else if (leftBuckets[i].ValueEnd.CompareTo(rightBuckets[j].ValueStart) >= 0)
                                match = true;
                            else
                                match = false;
                            break;
                    }
                    if (match)
                    {
                        List<Tuple<TableReferenceNode, string, BucketEstimate>> information = new List<Tuple<TableReferenceNode, string, BucketEstimate>>();
                        information.Add(Tuple.Create(predicate.LeftTable, predicate.LeftAttribute, new BucketEstimate(leftBuckets[i],
                            JoinCost.GetBucketEstimate(predicate.ComType, leftBuckets[i], rightBuckets[j]))));
                        information.Add(Tuple.Create(predicate.RightTable, predicate.RightAttribute, new BucketEstimate(rightBuckets[j],
                            JoinCost.GetBucketEstimate(predicate.ComType, rightBuckets[j], leftBuckets[i]))));
                        buckets.Add(new IntermediateBucket(information));
                    } 
                    else
                        rightCutoff = Math.Max(0, j - 1);
                }
            }
            return buckets;
        }

        private bool DoesMatch(IHistogramBucket leftBucket, IHistogramBucket rightBucket)
        {
            if ((rightBucket.ValueStart.CompareTo(leftBucket.ValueStart) >= 0 && rightBucket.ValueStart.CompareTo(leftBucket.ValueEnd) <= 0) ||
                (rightBucket.ValueEnd.CompareTo(leftBucket.ValueStart) >= 0 && rightBucket.ValueEnd.CompareTo(leftBucket.ValueEnd) <= 0))
                return true;
            return false;
        }
        #endregion
        #region Bounds
        private Tuple<IHistogramBucket[], IHistogramBucket[]> GetBucketBounds(JoinPredicate predicate, List<IHistogramBucket> leftBuckets, List<IHistogramBucket> rightBuckets)
        {
            int leftStart = 0;
            int leftEnd = leftBuckets.Count - 1;
            int rightStart = 0;
            int rightEnd = rightBuckets.Count - 1;

            if (leftBuckets[0].ValueStart.CompareTo(rightBuckets[0].ValueStart) < 0)
            {
                if (predicate.ComType != ComparisonType.Type.Less && predicate.ComType != ComparisonType.Type.EqualOrLess)
                    leftStart = GetMatchBucketIndex(leftBuckets, 0, leftBuckets.Count - 1, rightBuckets[0].ValueStart);
            }
            else if (leftBuckets[0].ValueStart.CompareTo(rightBuckets[0].ValueStart) > 0)
            {
                if (predicate.ComType != ComparisonType.Type.More && predicate.ComType != ComparisonType.Type.EqualOrMore)
                    rightStart = GetMatchBucketIndex(rightBuckets, 0, rightBuckets.Count - 1, leftBuckets[0].ValueStart);
            }

            if (leftBuckets[leftBuckets.Count - 1].ValueEnd.CompareTo(rightBuckets[rightBuckets.Count - 1].ValueEnd) > 0)
            {
                if (predicate.ComType != ComparisonType.Type.More && predicate.ComType != ComparisonType.Type.EqualOrMore)
                    leftEnd = GetMatchBucketIndex(leftBuckets, 0, leftBuckets.Count - 1, rightBuckets[rightBuckets.Count - 1].ValueEnd);
            }
            else if (leftBuckets[leftBuckets.Count - 1].ValueEnd.CompareTo(rightBuckets[rightBuckets.Count - 1].ValueEnd) < 0)
            {
                if (predicate.ComType != ComparisonType.Type.Less && predicate.ComType != ComparisonType.Type.EqualOrLess)
                    rightEnd = GetMatchBucketIndex(rightBuckets, 0, rightBuckets.Count - 1, leftBuckets[leftBuckets.Count - 1].ValueEnd);
            }

            if (leftStart == -1 || leftEnd == -1 || rightStart == -1 || rightEnd == -1)
                return new Tuple<IHistogramBucket[], IHistogramBucket[]>(new IHistogramBucket[] { }, new IHistogramBucket[] { });

            Range leftBucketMatchRange = new Range(leftStart, leftEnd + 1);
            Range rightBucketMatchRange = new Range(rightStart, rightEnd + 1);

            IHistogramBucket[] leftBucketMatch = leftBuckets.ToArray()[leftBucketMatchRange];
            IHistogramBucket[] rightBucketMatch = rightBuckets.ToArray()[rightBucketMatchRange];

            return new Tuple<IHistogramBucket[], IHistogramBucket[]>(leftBucketMatch, rightBucketMatch);
        }

        private int GetMatchBucketIndex(List<IHistogramBucket> buckets, int lowerIndexBound, int upperIndexBound, IComparable value)
        {
            if (upperIndexBound >= lowerIndexBound)
            {
                int mid = lowerIndexBound + (upperIndexBound - lowerIndexBound) / 2;

                if ((value.CompareTo(buckets[mid].ValueStart) >= 0) &&
                    (value.CompareTo(buckets[mid].ValueEnd) <= 0))
                    return mid;

                if (value.CompareTo(buckets[mid].ValueEnd) < 0)
                    return GetMatchBucketIndex(buckets, lowerIndexBound, mid - 1, value);

                return GetMatchBucketIndex(buckets, mid + 1, upperIndexBound, value);
            }

            return -1;
        }

        #endregion
        private Tuple<List<IHistogramBucket>, List<IHistogramBucket>> GetBucketPair(JoinPredicate node, IntermediateTable table)
        {
            if (table.DoesContain(node.LeftTable, node.LeftAttribute))
            {
                if (table.DoesContain(node.RightTable, node.RightAttribute))
                    return new Tuple<List<IHistogramBucket>, List<IHistogramBucket>>(
                        table.GetBuckets(node.LeftTable, node.LeftAttribute), 
                        table.GetBuckets(node.RightTable, node.RightAttribute));
                else
                    return new Tuple<List<IHistogramBucket>, List<IHistogramBucket>>(
                        table.GetBuckets(node.LeftTable, node.LeftAttribute), 
                        HistogramManager.GetHistogram(node.RightTable.TableName, node.RightAttribute).Buckets);
            }
            else if (table.DoesContain(node.RightTable, node.RightAttribute))
                return new Tuple<List<IHistogramBucket>, List<IHistogramBucket>>(
                        HistogramManager.GetHistogram(node.LeftTable.TableName, node.LeftAttribute).Buckets,
                        table.GetBuckets(node.RightTable, node.RightAttribute));
            else
                return new Tuple<List<IHistogramBucket>, List<IHistogramBucket>>(
                        HistogramManager.GetHistogram(node.LeftTable.TableName, node.LeftAttribute).Buckets,
                        HistogramManager.GetHistogram(node.RightTable.TableName, node.RightAttribute).Buckets);
        }

        private List<Tuple<TableReferenceNode, string>> GetOverlappingReferences(List<Tuple<TableReferenceNode, string>> leftReferences, List<Tuple<TableReferenceNode, string>> rightReferences)
        {
            List<Tuple<TableReferenceNode, string>> overlappingReferences = new List<Tuple<TableReferenceNode, string>>();
            foreach (var leftReference in leftReferences)
            {
                foreach (var rightReference in rightReferences)
                {
                    if (leftReference.Item1 == rightReference.Item1 && leftReference.Item2 == rightReference.Item2)
                    {
                        overlappingReferences.Add(leftReference);
                    }
                }
            }
            return overlappingReferences;
        }
    }
}
