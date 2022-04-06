using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QueryParser.Models;
using Histograms;
using Histograms.Models;
using DatabaseConnector;

namespace QueryOptimiser.Cost.Nodes
{
    public abstract class BaseJoinCost : INodeCost<JoinNode>
    {
        protected int GetMatchBucketIndex(List<IHistogramBucket> buckets, int lowerIndexBound, int upperIndexBound, IComparable value)
        {
            if (upperIndexBound >= lowerIndexBound)
            {
                int mid = lowerIndexBound + (upperIndexBound - lowerIndexBound) / 2;

                if (value.CompareTo(buckets[mid].ValueStart) >= 0 && value.CompareTo(buckets[mid].ValueEnd) <= 0)
                    return mid;

                if (value.CompareTo(buckets[mid].ValueEnd) < 0)
                    return GetMatchBucketIndex(buckets, lowerIndexBound, mid - 1, value);

                return GetMatchBucketIndex(buckets, mid + 1, upperIndexBound, value);
            }

            return -1;
        }

        public long CalculateCost(JoinNode node, List<IHistogramBucket> leftBuckets, List<IHistogramBucket> rightBuckets)
        {
            if (node.Relation != null)
            {
                if (node.Relation.Type == RelationType.Type.Predicate && node.Relation.LeafPredicate != null)
                    return CalculateCost(node.Relation.LeafPredicate, leftBuckets, rightBuckets);
                else if (node.Relation.Type == RelationType.Type.And || node.Relation.Type == RelationType.Type.Or)
                    return CalculateCost(node.Relation, leftBuckets, rightBuckets);
                else
                    throw new ArgumentException("Missing noderelation type " + node.Relation.ToString());
            }
            else
                throw new ArgumentException("node relation is not allowed to be null " + node.ToString());

        }

        private long CalculateCost(JoinPredicateRelation nodeRelation, List<IHistogramBucket> leftBuckets, List<IHistogramBucket> rightBuckets)
        {
            JoinPredicateRelation? leftRelation = nodeRelation.LeftRelation;
            JoinPredicateRelation? rightRelation = nodeRelation.RightRelation;

            if (leftRelation != null && rightRelation != null)
            {
                if (nodeRelation.Type == RelationType.Type.And)
                    return Math.Min(CalculateCost(leftRelation, leftBuckets, rightBuckets), CalculateCost(rightRelation, leftBuckets, rightBuckets));
                else if (nodeRelation.Type == RelationType.Type.Or)
                    return CalculateCost(leftRelation, leftBuckets, rightBuckets) + CalculateCost(rightRelation, leftBuckets, rightBuckets);
                else if (nodeRelation.Type == RelationType.Type.None)
                    throw new ArgumentNullException($"Noderelation type is not set {nodeRelation.ToString()}");
                else
                    throw new NotImplementedException($"The noderelation type of {nodeRelation.Type} is unhandled");
            }
            else if (nodeRelation.LeafPredicate != null)
            {
                return CalculateCost(nodeRelation.LeafPredicate, leftBuckets, rightBuckets);
            }
            else
                throw new ArgumentException("Missing noderelation type " + nodeRelation.ToString());
        }

        private long CalculateCost(JoinPredicate node, List<IHistogramBucket> leftBuckets, List<IHistogramBucket> rightBuckets)
        {
            int leftStart = 0;
            int leftEnd = leftBuckets.Count - 1;
            int rightStart = 0;
            int rightEnd = rightBuckets.Count - 1;

            if (leftBuckets[0].ValueStart.CompareTo(rightBuckets[0].ValueStart) < 0)
            {
                if (node.ComType != ComparisonType.Type.Less && node.ComType != ComparisonType.Type.EqualOrLess)
                    leftStart = GetMatchBucketIndex(leftBuckets, 0, leftBuckets.Count - 1, rightBuckets[0].ValueStart);
            }
            else if (leftBuckets[0].ValueStart.CompareTo(rightBuckets[0].ValueStart) > 0)
            {
                if (node.ComType != ComparisonType.Type.More && node.ComType != ComparisonType.Type.EqualOrMore)
                    rightStart = GetMatchBucketIndex(rightBuckets, 0, rightBuckets.Count - 1, leftBuckets[0].ValueStart);
            }

            if (leftBuckets[leftBuckets.Count - 1].ValueEnd.CompareTo(rightBuckets[rightBuckets.Count - 1].ValueEnd) > 0)
            {
                if (node.ComType != ComparisonType.Type.More && node.ComType != ComparisonType.Type.EqualOrMore)
                    leftEnd = GetMatchBucketIndex(leftBuckets, 0, leftBuckets.Count - 1, rightBuckets[rightBuckets.Count - 1].ValueEnd);
            }
            else if (leftBuckets[leftBuckets.Count - 1].ValueEnd.CompareTo(rightBuckets[rightBuckets.Count - 1].ValueEnd) < 0)
            {
                if (node.ComType != ComparisonType.Type.Less && node.ComType != ComparisonType.Type.EqualOrLess)
                    rightEnd = GetMatchBucketIndex(rightBuckets, 0, rightBuckets.Count - 1, leftBuckets[leftBuckets.Count - 1].ValueEnd);
            }

            if (leftStart == -1 || leftEnd == -1 || rightStart == -1 || rightEnd == -1)
                return 0;

            // This is a tad bit of symptom fixing rather than actual fix
            if (node.ComType == ComparisonType.Type.Less && leftBuckets[leftEnd].ValueEnd.CompareTo(rightBuckets[rightStart].ValueStart) == 0)
                return 0;
            if (node.ComType == ComparisonType.Type.More && leftBuckets[leftStart].ValueEnd.CompareTo(rightBuckets[rightEnd].ValueEnd) == 0)
                return 0;

            Range leftMatch = new Range(leftStart, leftEnd + 1);
            Range rightMatch = new Range(rightStart, rightEnd + 1);

            IHistogramBucket[] leftBucketMatch = leftBuckets.ToArray()[leftMatch];
            IHistogramBucket[] rightBucketMatch = rightBuckets.ToArray()[rightMatch];

            return CalculateCost(node.ComType, leftBucketMatch, rightBucketMatch);
        }

        protected abstract long CalculateCost(ComparisonType.Type predicate, IHistogramBucket[] leftBuckets, IHistogramBucket[] rightBuckets);
    }
}
