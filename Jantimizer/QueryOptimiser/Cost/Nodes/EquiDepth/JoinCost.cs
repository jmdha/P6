using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QueryParser.Models;
using Histograms;
using DatabaseConnector;

namespace QueryOptimiser.Cost.Nodes.EquiDepth
{
    public class JoinCost : INodeCostEquiDepth<JoinNode, IDbConnector>
    {
        public int CalculateCost(JoinNode node, IHistogramManager<IHistogram, IDbConnector> histogramManager)
        {
            if (node.Relation != null) {
                if (node.Relation.Type == JoinPredicateRelation.RelationType.Predicate && node.Relation.LeafPredicate != null)
                    return CalculateCost(node.Relation.LeafPredicate, histogramManager);
                else if (node.Relation.Type == JoinPredicateRelation.RelationType.And || node.Relation.Type == JoinPredicateRelation.RelationType.Or)
                    return CalculateCost(node.Relation, histogramManager);
                else
                    throw new ArgumentException("Missing noderelation type " + node.Relation.ToString());
            } else
                throw new ArgumentException("node relation is not allowed to be null " + node.ToString());
                
        }

        public int CalculateCost(JoinPredicateRelation nodeRelation, IHistogramManager<IHistogram, IDbConnector> histogramManager) {
            JoinPredicateRelation? leftRelation = nodeRelation.LeftRelation;
            JoinPredicateRelation? rightRelation = nodeRelation.RightRelation;

            switch (nodeRelation.Type) {
                case JoinPredicateRelation.RelationType.Predicate:
                    return CalculateCost(nodeRelation.LeafPredicate, histogramManager);
                case JoinPredicateRelation.RelationType.And:
                    return Math.Min(CalculateCost(leftRelation, histogramManager), CalculateCost(rightRelation, histogramManager));
                case JoinPredicateRelation.RelationType.Or:
                    return CalculateCost(leftRelation, histogramManager) + CalculateCost(rightRelation, histogramManager);
            }
            throw new ArgumentException("Missing noderelation type " + nodeRelation.ToString());
        }

        public int CalculateCost(JoinPredicate node, IHistogramManager<IHistogram, IDbConnector> histogramManager) {
            IHistogram leftGram = histogramManager.GetHistogram(node.LeftTable.Alias, node.LeftAttribute);
            IHistogram rightGram = histogramManager.GetHistogram(node.RightTable.Alias, node.RightAttribute);
            int leftBucketStart = -1;
            int leftBucketEnd = -1;
            int rightBucketStart = -1;
            int rightBucketEnd = -1;

            for (int leftBucketIndex = 0; leftBucketIndex < leftGram.Buckets.Count; leftBucketIndex++)
            {
                for (int rightBucketIndex = 0; rightBucketIndex < rightGram.Buckets.Count; rightBucketIndex++)
                {
                    bool inBounds = false;
                    switch (node.ComType)
                    {
                        case ComparisonType.Type.Equal:
                            inBounds = CheckEqualBounds(
                                leftGram.Buckets[leftBucketIndex].ValueStart,
                                rightGram.Buckets[rightBucketIndex].ValueStart,
                                leftGram.Buckets[leftBucketIndex].ValueEnd,
                                rightGram.Buckets[rightBucketIndex].ValueEnd);
                            break;
                        case ComparisonType.Type.Less:
                            inBounds = CheckLessBounds(
                                leftGram.Buckets[leftBucketIndex].ValueStart,
                                rightGram.Buckets[rightBucketIndex].ValueStart,
                                leftGram.Buckets[leftBucketIndex].ValueEnd,
                                rightGram.Buckets[rightBucketIndex].ValueEnd);
                            break;
                        case ComparisonType.Type.More:
                            inBounds = CheckMoreBounds(
                                leftGram.Buckets[leftBucketIndex].ValueStart,
                                rightGram.Buckets[rightBucketIndex].ValueStart,
                                leftGram.Buckets[leftBucketIndex].ValueEnd,
                                rightGram.Buckets[rightBucketIndex].ValueEnd);
                            break;
                        case ComparisonType.Type.EqualOrLess:
                            inBounds = CheckEqualOrLessBounds(
                                leftGram.Buckets[leftBucketIndex].ValueStart,
                                rightGram.Buckets[rightBucketIndex].ValueStart,
                                leftGram.Buckets[leftBucketIndex].ValueEnd,
                                rightGram.Buckets[rightBucketIndex].ValueEnd);
                            break;
                        case ComparisonType.Type.EqualOrMore:
                            inBounds = CheckEqualOrMoreBounds(
                                leftGram.Buckets[leftBucketIndex].ValueStart,
                                rightGram.Buckets[rightBucketIndex].ValueStart,
                                leftGram.Buckets[leftBucketIndex].ValueEnd,
                                rightGram.Buckets[rightBucketIndex].ValueEnd);
                            break;
                        case ComparisonType.Type.None:
                            throw new ArgumentException("No join type specified : " + node.ToString());
                        default:
                            throw new ArgumentException("Unhandled join type: " + node.ToString());
                    }

                    if (inBounds)
                    {
                        if (leftBucketStart == -1)
                            leftBucketStart = leftBucketIndex;
                        if (leftBucketEnd < leftBucketIndex)
                            leftBucketEnd = leftBucketIndex;
                        if (rightBucketStart == -1)
                            rightBucketStart = rightBucketIndex;
                        if (rightBucketEnd < rightBucketIndex)
                            rightBucketEnd = rightBucketIndex;
                    }
                }
            }   

            // No overlap
            if (leftBucketStart == -1 || leftBucketEnd == -1 || rightBucketEnd == -1 || rightBucketEnd == -1)
                return 0;

            int leftBucketCount = 0;
            int rightBucketCount = 0;

            for (int i = leftBucketStart; i <= leftBucketEnd; i++)
                leftBucketCount += leftGram.Buckets[i].Count;
            for (int i = rightBucketStart; i <= rightBucketEnd; i++)
                rightBucketCount += rightGram.Buckets[i].Count;

            return leftBucketCount * rightBucketCount;
        }

        private static bool CheckEqualBounds(int leftValueStart, int rightValueStart, int leftValueEnd, int rightValueEnd)
        {
            return ((rightValueStart >= leftValueStart &&
                         rightValueStart <= leftValueEnd) ||
                        (rightValueEnd <= leftValueEnd &&
                         rightValueEnd >= leftValueStart));
        }

        private static bool CheckLessBounds(int leftValueStart, int rightValueStart, int leftValueEnd, int rightValueEnd)
        {
            return leftValueStart < rightValueEnd;
        }

        private static bool CheckMoreBounds(int leftValueStart, int rightValueStart, int leftValueEnd, int rightValueEnd)
        {
            return leftValueEnd > rightValueStart;
        }

        private static bool CheckEqualOrLessBounds(int leftValueStart, int rightValueStart, int leftValueEnd, int rightValueEnd)
        {
            return (
                CheckEqualBounds(leftValueStart, rightValueStart, leftValueEnd, rightValueEnd) ||
                CheckLessBounds(leftValueStart, rightValueStart, leftValueEnd, rightValueEnd));
        }

        private static bool CheckEqualOrMoreBounds(int leftValueStart, int rightValueStart, int leftValueEnd, int rightValueEnd)
        {
            return (
                CheckEqualBounds(leftValueStart, rightValueStart, leftValueEnd, rightValueEnd) ||
                CheckMoreBounds(leftValueStart, rightValueStart, leftValueEnd, rightValueEnd));
        }

        
    }
}
