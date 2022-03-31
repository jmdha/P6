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
        protected int GetMatchBucketIndex(IHistogram gram, int lowerIndexBound, int upperIndexBound, IComparable value)
        {
            if (upperIndexBound >= lowerIndexBound)
            {
                int mid = lowerIndexBound + (upperIndexBound - lowerIndexBound) / 2;

                if (value.CompareTo(gram.Buckets[mid].ValueStart) >= 0 && value.CompareTo(gram.Buckets[mid].ValueEnd) <= 0)
                    return mid;

                if (value.CompareTo(gram.Buckets[mid].ValueEnd) < 0)
                    return GetMatchBucketIndex(gram, lowerIndexBound, mid - 1, value);

                return GetMatchBucketIndex(gram, mid + 1, upperIndexBound, value);
            }

            return -1;
        }

        public long CalculateCost(JoinNode node, IHistogramManager histogramManager)
        {
            if (node.Relation != null)
            {
                if (node.Relation.Type == JoinPredicateRelation.RelationType.Predicate && node.Relation.LeafPredicate != null)
                    return CalculateCost(node.Relation.LeafPredicate, histogramManager);
                else if (node.Relation.Type == JoinPredicateRelation.RelationType.And || node.Relation.Type == JoinPredicateRelation.RelationType.Or)
                    return CalculateCost(node.Relation, histogramManager);
                else
                    throw new ArgumentException("Missing noderelation type " + node.Relation.ToString());
            }
            else
                throw new ArgumentException("node relation is not allowed to be null " + node.ToString());

        }

        private long CalculateCost(JoinPredicateRelation nodeRelation, IHistogramManager histogramManager)
        {
            JoinPredicateRelation? leftRelation = nodeRelation.LeftRelation;
            JoinPredicateRelation? rightRelation = nodeRelation.RightRelation;

            if (leftRelation != null && rightRelation != null)
            {
                if (nodeRelation.Type == JoinPredicateRelation.RelationType.And)
                    return Math.Min(CalculateCost(leftRelation, histogramManager), CalculateCost(rightRelation, histogramManager));
                else if (nodeRelation.Type == JoinPredicateRelation.RelationType.Or)
                    return CalculateCost(leftRelation, histogramManager) + CalculateCost(rightRelation, histogramManager);
                else if (nodeRelation.Type == JoinPredicateRelation.RelationType.None)
                    throw new ArgumentNullException($"Noderelation type is not set {nodeRelation.ToString()}");
                else
                    throw new NotImplementedException($"The noderelation type of {nodeRelation.Type} is unhandled");
            }
            else if (nodeRelation.LeafPredicate != null)
            {
                return CalculateCost(nodeRelation.LeafPredicate, histogramManager);
            }
            else
                throw new ArgumentException("Missing noderelation type " + nodeRelation.ToString());
        }

        private long CalculateCost(JoinPredicate node, IHistogramManager histogramManager)
        {
            IHistogram leftGram = histogramManager.GetHistogram(node.LeftTable.Alias, node.LeftAttribute);
            IHistogram rightGram = histogramManager.GetHistogram(node.RightTable.Alias, node.RightAttribute);
            int leftStart = 0;
            int leftEnd = leftGram.Buckets.Count - 1;
            int rightStart = 0;
            int rightEnd = rightGram.Buckets.Count - 1;

            if (leftGram.Buckets[0].ValueStart.CompareTo(rightGram.Buckets[0].ValueStart) < 0)
            {
                if (node.ComType != ComparisonType.Type.Less && node.ComType != ComparisonType.Type.EqualOrLess)
                    leftStart = GetMatchBucketIndex(leftGram, 0, leftGram.Buckets.Count - 1, rightGram.Buckets[0].ValueStart);
            }
            else if (leftGram.Buckets[0].ValueStart.CompareTo(rightGram.Buckets[0].ValueStart) > 0)
            {
                if (node.ComType != ComparisonType.Type.More && node.ComType != ComparisonType.Type.EqualOrMore)
                    rightStart = GetMatchBucketIndex(rightGram, 0, rightGram.Buckets.Count - 1, leftGram.Buckets[0].ValueStart);
            }

            if (leftGram.Buckets[leftGram.Buckets.Count - 1].ValueEnd.CompareTo(rightGram.Buckets[rightGram.Buckets.Count - 1].ValueEnd) > 0)
            {
                if (node.ComType != ComparisonType.Type.More && node.ComType != ComparisonType.Type.EqualOrMore)
                    leftEnd = GetMatchBucketIndex(leftGram, 0, leftGram.Buckets.Count - 1, rightGram.Buckets[rightGram.Buckets.Count - 1].ValueEnd);
            }
            else if (leftGram.Buckets[leftGram.Buckets.Count - 1].ValueEnd.CompareTo(rightGram.Buckets[rightGram.Buckets.Count - 1].ValueEnd) < 0)
            {
                if (node.ComType != ComparisonType.Type.Less && node.ComType != ComparisonType.Type.EqualOrLess)
                    rightEnd = GetMatchBucketIndex(rightGram, 0, rightGram.Buckets.Count - 1, leftGram.Buckets[leftGram.Buckets.Count - 1].ValueEnd);
            }

            if (leftStart == -1 || leftEnd == -1 || rightStart == -1 || rightEnd == -1)
                return 0;

            // This is a tad bit of symptom fixing rather than actual fix
            if (node.ComType == ComparisonType.Type.Less && leftGram.Buckets[leftEnd].ValueEnd.CompareTo(rightGram.Buckets[rightStart].ValueStart) == 0)
                return 0;
            if (node.ComType == ComparisonType.Type.More && leftGram.Buckets[leftStart].ValueEnd.CompareTo(rightGram.Buckets[rightEnd].ValueEnd) == 0)
                return 0;

            Range leftBucketMatch = new Range(leftStart, leftEnd + 1);
            Range rightBucketMatch = new Range(rightStart, rightEnd + 1);

            IHistogramBucket[] leftBuckets = leftGram.Buckets.ToArray()[leftBucketMatch];
            IHistogramBucket[] rightBuckets = rightGram.Buckets.ToArray()[rightBucketMatch];

            return CalculateCost(node.ComType, leftBuckets, rightBuckets);
        }

        protected abstract long CalculateCost(ComparisonType.Type predicate, IHistogramBucket[] leftBuckets, IHistogramBucket[] rightBuckets);
    }
}
