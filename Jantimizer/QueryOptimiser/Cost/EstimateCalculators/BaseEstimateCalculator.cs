using Histograms;
using Histograms.Models;
using QueryOptimiser.Cost.Nodes;
using QueryOptimiser.Helpers;
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
                return EstimateJoinTable(joinNode, intermediateTable);
            else
                throw new ArgumentException("Non handled node type " + node.ToString());
        }

        internal IntermediateTable EstimateJoinTable(JoinNode node, IntermediateTable table)
        {
            if (node.Relation == null)
                throw new ArgumentException("node relation is not allowed to be null " + node.ToString());

            List<IntermediateBucket> buckets;
            List<TableAttribute> references = new List<TableAttribute>();

            buckets = GetBucketMatches(node.Relation, table, ref references);

            return new IntermediateTable(buckets, references);
        }

        #region Matches

        internal List<IntermediateBucket> GetBucketMatches(JoinPredicateRelation relation, IntermediateTable table, ref List<TableAttribute> references)
        {
            if (relation.Type == RelationType.Type.Predicate && relation.LeafPredicate != null)
                return GetBucketMatches(relation.LeafPredicate, table, ref references);
            else if (relation.LeftRelation != null && relation.RightRelation != null)
            {
                List<TableAttribute> leftReferences = new List<TableAttribute>();
                List<TableAttribute> rightReferences = new List<TableAttribute>();
                List<IntermediateBucket> leftBuckets = GetBucketMatches(relation.LeftRelation, table, ref leftReferences);
                List<IntermediateBucket> rightBuckets = GetBucketMatches(relation.RightRelation, table, ref rightReferences);

                List<IntermediateBucket> overlap = new List<IntermediateBucket>();
                if (relation.Type == RelationType.Type.And)
                {
                    overlap = BucketHelper.MergeOnOverlap(leftBuckets, rightBuckets);
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

        internal List<IntermediateBucket> GetBucketMatches(JoinPredicate predicate, IntermediateTable table, ref List<TableAttribute> references)
        {
            Tuple<List<IHistogramBucket>, List<IHistogramBucket>> bucketPair = GetBucketPair(predicate, table);
            Tuple<IHistogramBucket[], IHistogramBucket[]> bounds = GetBucketBounds(predicate, bucketPair.Item1, bucketPair.Item2);

            if (bounds.Item1.Length > 0 && bounds.Item2.Length > 0) {
                references.Add(new TableAttribute(predicate.LeftTable.Alias, predicate.LeftAttribute));
                references.Add(new TableAttribute(predicate.RightTable.Alias, predicate.RightAttribute));
            } else
                return new List<IntermediateBucket>();

            if (predicate.ComType == ComparisonType.Type.Equal)
                return GetEqualityMatches(predicate, bounds.Item1, bounds.Item2);
            else
                return GetInEqualityMatches(predicate, bounds.Item1, bounds.Item2);
        }

        internal List<IntermediateBucket> GetEqualityMatches(JoinPredicate predicate, IHistogramBucket[] leftBuckets, IHistogramBucket[] rightBuckets)
        {
            List<IntermediateBucket> buckets = new List<IntermediateBucket>();
            int rightCutoff = 0;
            for (int i = 0; i < leftBuckets.Length; i++)
            {
                for (int j = rightCutoff; j < rightBuckets.Length; j++)
                {
                    if (DoesMatch(leftBuckets[i], rightBuckets[j]))
                        buckets.Add(MakeNewIntermediateBucket(predicate, leftBuckets[i], rightBuckets[j]));
                    else
                        rightCutoff = Math.Max(0, j - 1);
                }
            }
            return buckets;
        }

        internal IntermediateBucket MakeNewIntermediateBucket(JoinPredicate predicate, IHistogramBucket leftBucket, IHistogramBucket rightBucket)
        {
            var newBucket = new IntermediateBucket();
            newBucket.AddBucket(
                new TableAttribute(predicate.LeftTable.Alias, predicate.LeftAttribute),
                new BucketEstimate(
                    leftBucket,
                    JoinCost.GetBucketEstimate(predicate.ComType, leftBucket, rightBucket)
                    )
                );
            newBucket.AddBucket(
                new TableAttribute(predicate.RightTable.Alias, predicate.RightAttribute),
                new BucketEstimate(
                    rightBucket,
                    JoinCost.GetBucketEstimate(predicate.ComType, rightBucket, leftBucket)
                    )
                );
            return newBucket;
        }

        internal List<IntermediateBucket> GetInEqualityMatches(JoinPredicate predicate, IHistogramBucket[] leftBuckets, IHistogramBucket[] rightBuckets)
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
                        buckets.Add(MakeNewIntermediateBucket(predicate, leftBuckets[i], rightBuckets[j]));
                    else
                        rightCutoff = Math.Max(0, j - 1);
                }
            }
            return buckets;
        }

        internal bool DoesMatch(IHistogramBucket leftBucket, IHistogramBucket rightBucket)
        {
            if ((rightBucket.ValueStart.CompareTo(leftBucket.ValueStart) >= 0 && rightBucket.ValueStart.CompareTo(leftBucket.ValueEnd) <= 0) ||
                (rightBucket.ValueEnd.CompareTo(leftBucket.ValueStart) >= 0 && rightBucket.ValueEnd.CompareTo(leftBucket.ValueEnd) <= 0))
                return true;
            return false;
        }
        #endregion
        #region Bounds
        internal Tuple<IHistogramBucket[], IHistogramBucket[]> GetBucketBounds(JoinPredicate predicate, List<IHistogramBucket> leftBuckets, List<IHistogramBucket> rightBuckets)
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

        internal int GetMatchBucketIndex(List<IHistogramBucket> buckets, int lowerIndexBound, int upperIndexBound, IComparable value)
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
        internal Tuple<List<IHistogramBucket>, List<IHistogramBucket>> GetBucketPair(JoinPredicate node, IntermediateTable table)
        {
            var leftTableAttribute = new TableAttribute(node.LeftTable.Alias, node.LeftAttribute);
            var rightTableAttribute = new TableAttribute(node.RightTable.Alias, node.RightAttribute);

            if (table.References.Contains(leftTableAttribute))
            {
                if (table.References.Contains(rightTableAttribute))
                    return new Tuple<List<IHistogramBucket>, List<IHistogramBucket>>(
                        table.GetBuckets(leftTableAttribute), 
                        table.GetBuckets(rightTableAttribute));
                else
                    return new Tuple<List<IHistogramBucket>, List<IHistogramBucket>>(
                        table.GetBuckets(leftTableAttribute), 
                        HistogramManager.GetHistogram(node.RightTable.TableName, node.RightAttribute).Buckets);
            }
            else if (table.References.Contains(rightTableAttribute))
                return new Tuple<List<IHistogramBucket>, List<IHistogramBucket>>(
                        HistogramManager.GetHistogram(node.LeftTable.TableName, node.LeftAttribute).Buckets,
                        table.GetBuckets(rightTableAttribute));
            else
                return new Tuple<List<IHistogramBucket>, List<IHistogramBucket>>(
                        HistogramManager.GetHistogram(node.LeftTable.TableName, node.LeftAttribute).Buckets,
                        HistogramManager.GetHistogram(node.RightTable.TableName, node.RightAttribute).Buckets);
        }

        internal List<TableAttribute> GetOverlappingReferences(List<TableAttribute> leftReferences, List<TableAttribute> rightReferences)
        {
            List<TableAttribute> overlappingReferences = new List<TableAttribute>();
            foreach (var leftReference in leftReferences)
                foreach (var rightReference in rightReferences)
                    if (leftReference.Equals(rightReference))
                        overlappingReferences.Add(leftReference);
            return overlappingReferences;
        }
    }
}
