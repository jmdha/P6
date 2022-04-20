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
using Tools.Helpers;

namespace QueryOptimiser.Cost.EstimateCalculators
{
    public abstract class BaseEstimateCalculator : IEstimateCalculator
    {
        public IHistogramManager HistogramManager { get; set; }
        public abstract INodeCost<JoinNode> NodeCostCalculator { get; set; }

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

            BucketMatches bucketMatches = GetBucketMatches(node.Relation, table);
            return new IntermediateTable(bucketMatches.Buckets, bucketMatches.References);
        }

        #region Matches

        internal BucketMatches GetBucketMatches(JoinPredicateRelation relation, IntermediateTable table)
        {
            if (relation.Type == RelationType.Type.Predicate && relation.LeafPredicate != null)
                return GetBucketMatches(relation.LeafPredicate, table);
            else if (relation.LeftRelation != null && relation.RightRelation != null)
            {
                BucketMatches leftMatches = GetBucketMatches(relation.LeftRelation, table);
                BucketMatches rightMatches = GetBucketMatches(relation.RightRelation, table);

                BucketMatches overlap = new BucketMatches();
                if (relation.Type == RelationType.Type.And)
                {
                    overlap.Buckets = BucketHelper.MergeOnOverlap(leftMatches.Buckets, rightMatches.Buckets);
                    overlap.References = GetOverlappingReferences(leftMatches.References, rightMatches.References);
                }
                else if (relation.Type == RelationType.Type.Or)
                {
                    overlap.Buckets.AddRange(leftMatches.Buckets);
                    overlap.Buckets.AddRange(rightMatches.Buckets);
                    overlap.References.AddRange(leftMatches.References);
                    overlap.References.AddRange(rightMatches.References);
                }
                
                return overlap;
            }
            else
                throw new ArgumentException("Missing noderelation type " + relation.ToString());
        }

        internal BucketMatches GetBucketMatches(JoinPredicate predicate, IntermediateTable table)
        {
            BucketMatches returnMatches = new BucketMatches();
            PairBucketList bucketPair = GetBucketPair(
                new TableAttribute(predicate.LeftTable.TableName, predicate.LeftAttribute),
                new TableAttribute(predicate.RightTable.TableName, predicate.RightAttribute),
                table);
            PairBucketList bounds = GetBucketBounds(predicate.ComType, bucketPair.LeftBuckets, bucketPair.RightBuckets);

            if (bounds.LeftBuckets.Count > 0 && bounds.RightBuckets.Count > 0) {
                returnMatches.References.Add(new TableAttribute(predicate.LeftTable.TableName, predicate.LeftAttribute));
                returnMatches.References.Add(new TableAttribute(predicate.RightTable.TableName, predicate.RightAttribute));
            } else
                return returnMatches;

            if (predicate.ComType == ComparisonType.Type.Equal)
                returnMatches.Buckets = GetEqualityMatches(predicate, bounds.LeftBuckets, bounds.RightBuckets);
            else
                returnMatches.Buckets = GetInEqualityMatches(predicate, bounds.LeftBuckets, bounds.RightBuckets);
            return returnMatches;
        }

        internal List<IntermediateBucket> GetEqualityMatches(JoinPredicate predicate, List<IHistogramBucket> leftBuckets, List<IHistogramBucket> rightBuckets)
        {
            List<IntermediateBucket> buckets = new List<IntermediateBucket>();
            int rightCutoff = 0;
            for (int i = 0; i < leftBuckets.Count; i++)
            {
                for (int j = rightCutoff; j < rightBuckets.Count; j++)
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
            newBucket.AddBucketIfNotThere(
                new TableAttribute(predicate.LeftTable.TableName, predicate.LeftAttribute),
                new BucketEstimate(
                    leftBucket,
                    NodeCostCalculator.GetBucketEstimate(predicate.ComType, leftBucket, rightBucket)
                    )
                );
            newBucket.AddBucketIfNotThere(
                new TableAttribute(predicate.RightTable.TableName, predicate.RightAttribute),
                new BucketEstimate(
                    rightBucket,
                    NodeCostCalculator.GetBucketEstimate(predicate.ComType, rightBucket, leftBucket)
                    )
                );
            return newBucket;
        }

        internal List<IntermediateBucket> GetInEqualityMatches(JoinPredicate predicate, List<IHistogramBucket> leftBuckets, List<IHistogramBucket> rightBuckets)
        {
            List<IntermediateBucket> buckets = new List<IntermediateBucket>();
            int rightCutoff = rightBuckets.Count - 1;
            for (int i = leftBuckets.Count - 1; i >= 0; i--)
            {
                for (int j = rightCutoff; j >= 0; j--)
                {
                    bool match = true;
                    switch (predicate.ComType)
                    {
                        case ComparisonType.Type.Less:
                            if (leftBuckets[i].ValueEnd.IsLessThan(rightBuckets[j].ValueStart))
                                match = true;
                            else if (leftBuckets[i].ValueStart.IsLessThan(rightBuckets[j].ValueEnd))
                                match = true;
                            else
                                match = false;
                            break;
                        case ComparisonType.Type.EqualOrLess:
                            if (leftBuckets[i].ValueEnd.IsLessThanOrEqual(rightBuckets[j].ValueStart))
                                match = true;
                            else if (leftBuckets[i].ValueStart.IsLessThanOrEqual(rightBuckets[j].ValueEnd))
                                match = true;
                            else
                                match = false;
                            break;
                        case ComparisonType.Type.More:
                            if (leftBuckets[i].ValueStart.IsLargerThan(rightBuckets[j].ValueEnd))
                                match = true;
                            else if (leftBuckets[i].ValueEnd.IsLargerThan(rightBuckets[j].ValueStart))
                                match = true;
                            else
                                match = false;
                            break;
                        case ComparisonType.Type.EqualOrMore:
                            if (leftBuckets[i].ValueStart.IsLargerThanOrEqual(rightBuckets[j].ValueEnd))
                                match = true;
                            else if (leftBuckets[i].ValueEnd.IsLargerThanOrEqual(rightBuckets[j].ValueStart))
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
            // Right bucket start index is within Left bucket range
            // Right Bucket:      |======|
            // Left Bucket:    |======|
            if (rightBucket.ValueStart.IsLargerThanOrEqual(leftBucket.ValueStart) && rightBucket.ValueStart.IsLessThanOrEqual(leftBucket.ValueEnd))
                return true;
            // Right bucket end index is within Left bucket range
            // Right Bucket:   |======|
            // Left Bucket:       |======|
            if (rightBucket.ValueEnd.IsLargerThanOrEqual(leftBucket.ValueStart) && rightBucket.ValueEnd.IsLessThanOrEqual(leftBucket.ValueEnd))
                return true;
            // Right bucket is entirely within Left bucket
            // Right Bucket:   |======|
            // Left Bucket:  |===========|
            if (rightBucket.ValueStart.IsLargerThanOrEqual(leftBucket.ValueStart) && rightBucket.ValueEnd.IsLessThanOrEqual(leftBucket.ValueEnd))
                return true;
            // Left bucket is entirely within Right bucket
            // Right Bucket: |===========|
            // Left Bucket:     |=====|
            if (rightBucket.ValueStart.IsLessThanOrEqual(leftBucket.ValueStart) && rightBucket.ValueEnd.IsLargerThanOrEqual(leftBucket.ValueEnd))
                return true;
            return false;
        }

        #endregion

        #region Bounds

        internal PairBucketList GetBucketBounds(ComparisonType.Type predicateType, List<IHistogramBucket> leftBuckets, List<IHistogramBucket> rightBuckets)
        {
            int leftStart = 0;
            int leftEnd = leftBuckets.Count - 1;
            int rightStart = 0;
            int rightEnd = rightBuckets.Count - 1;

            if (leftBuckets[0].ValueStart.IsLessThan(rightBuckets[0].ValueStart))
            {
                if (predicateType != ComparisonType.Type.Less && predicateType != ComparisonType.Type.EqualOrLess)
                    leftStart = GetMatchBucketIndex(leftBuckets, 0, leftBuckets.Count - 1, rightBuckets[0].ValueStart);
            }
            else if (leftBuckets[0].ValueStart.IsLargerThan(rightBuckets[0].ValueStart))
            {
                if (predicateType != ComparisonType.Type.More && predicateType != ComparisonType.Type.EqualOrMore)
                    rightStart = GetMatchBucketIndex(rightBuckets, 0, rightBuckets.Count - 1, leftBuckets[0].ValueStart);
            }

            if (leftBuckets[leftBuckets.Count - 1].ValueEnd.IsLargerThan(rightBuckets[rightBuckets.Count - 1].ValueEnd))
            {
                if (predicateType != ComparisonType.Type.More && predicateType != ComparisonType.Type.EqualOrMore)
                    leftEnd = GetMatchBucketIndex(leftBuckets, 0, leftBuckets.Count - 1, rightBuckets[rightBuckets.Count - 1].ValueEnd);
            }
            else if (leftBuckets[leftBuckets.Count - 1].ValueEnd.IsLessThan(rightBuckets[rightBuckets.Count - 1].ValueEnd))
            {
                if (predicateType != ComparisonType.Type.Less && predicateType != ComparisonType.Type.EqualOrLess)
                    rightEnd = GetMatchBucketIndex(rightBuckets, 0, rightBuckets.Count - 1, leftBuckets[leftBuckets.Count - 1].ValueEnd);
            }

            if (leftStart == -1 || leftEnd == -1 || rightStart == -1 || rightEnd == -1)
                return new PairBucketList();

            Range leftBucketMatchRange = new Range(leftStart, leftEnd + 1);
            Range rightBucketMatchRange = new Range(rightStart, rightEnd + 1);

            return new PairBucketList(leftBuckets.GetRange(leftBucketMatchRange), rightBuckets.GetRange(rightBucketMatchRange));
        }

        internal int GetMatchBucketIndex(List<IHistogramBucket> buckets, int lowerIndexBound, int upperIndexBound, IComparable value)
        {
            if (upperIndexBound >= lowerIndexBound)
            {
                int mid = lowerIndexBound + (upperIndexBound - lowerIndexBound) / 2;

                if ((value.IsLargerThanOrEqual(buckets[mid].ValueStart)) &&
                    (value.IsLessThanOrEqual(buckets[mid].ValueEnd)))
                    return mid;

                if (value.IsLessThan(buckets[mid].ValueEnd))
                    return GetMatchBucketIndex(buckets, lowerIndexBound, mid - 1, value);

                return GetMatchBucketIndex(buckets, mid + 1, upperIndexBound, value);
            }

            return -1;
        }

        #endregion

        internal PairBucketList GetBucketPair(TableAttribute leftTableAttribute, TableAttribute rightTableAttribute, IntermediateTable table)
        {
            if (table.References.Contains(leftTableAttribute))
            {
                if (table.References.Contains(rightTableAttribute))
                    return new PairBucketList(
                        table.GetBuckets(leftTableAttribute), 
                        table.GetBuckets(rightTableAttribute));
                else
                    return new PairBucketList(
                        table.GetBuckets(leftTableAttribute),
                        GetHistogram(rightTableAttribute).Buckets);
            }
            else if (table.References.Contains(rightTableAttribute))
                return new PairBucketList(
                        GetHistogram(leftTableAttribute).Buckets,
                        table.GetBuckets(rightTableAttribute));
            else
                return new PairBucketList(
                        GetHistogram(leftTableAttribute).Buckets,
                        GetHistogram(rightTableAttribute).Buckets);
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

        internal IHistogram GetHistogram(TableAttribute tableAttribute)
        {
            return HistogramManager.GetHistogram(tableAttribute.Table, tableAttribute.Attribute);
        }
    }
}
