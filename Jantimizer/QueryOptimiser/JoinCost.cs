using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QueryParser.Models;
using Histograms;
using DatabaseConnector;

namespace QueryOptimiser
{
    public static class JoinCost
    {
        /// <summary>
        /// Gives an estimate of the cost of a join operation
        /// <para>Specifically it gives the worst case cardinality estimate</para>
        /// </summary>
        /// <returns></returns>
        public static int CalculateJoinCost(JoinNode joinNode, Histograms.Managers.PostgresEquiDepthHistogramManager  histograms) {
            IHistogram leftGram = histograms.GetHistogram(joinNode.LeftTable, joinNode.LeftAttribute);
            IHistogram rightGram = histograms.GetHistogram(joinNode.RightTable, joinNode.RightAttribute);
            int leftBucketStart = -1;
            int leftBucketEnd = -1;
            int rightBucketStart = -1;
            int rightBucketEnd = -1;

            for (int leftBucketIndex = 0; leftBucketIndex < leftGram.Buckets.Count; leftBucketIndex++)
            {
                for (int rightBucketIndex = 0; rightBucketIndex < rightGram.Buckets.Count; rightBucketIndex++)
                {
                    bool inBounds = false;
                    switch (joinNode.ComType)
                    {
                        case JoinNode.ComparisonType.Equal:
                            inBounds = CheckEqualBounds(
                                leftGram.Buckets[leftBucketIndex].ValueStart,
                                rightGram.Buckets[rightBucketIndex].ValueStart,
                                leftGram.Buckets[leftBucketIndex].ValueEnd,
                                rightGram.Buckets[rightBucketIndex].ValueEnd);
                            break;
                        case JoinNode.ComparisonType.Less:
                            inBounds = CheckLessBounds(
                                leftGram.Buckets[leftBucketIndex].ValueStart,
                                rightGram.Buckets[rightBucketIndex].ValueStart,
                                leftGram.Buckets[leftBucketIndex].ValueEnd,
                                rightGram.Buckets[rightBucketIndex].ValueEnd);
                            break;
                        case JoinNode.ComparisonType.More:
                            inBounds = CheckMoreBounds(
                                leftGram.Buckets[leftBucketIndex].ValueStart,
                                rightGram.Buckets[rightBucketIndex].ValueStart,
                                leftGram.Buckets[leftBucketIndex].ValueEnd,
                                rightGram.Buckets[rightBucketIndex].ValueEnd);
                            break;
                        case JoinNode.ComparisonType.EqualOrLess:
                            inBounds = CheckEqualOrLessBounds(
                                leftGram.Buckets[leftBucketIndex].ValueStart,
                                rightGram.Buckets[rightBucketIndex].ValueStart,
                                leftGram.Buckets[leftBucketIndex].ValueEnd,
                                rightGram.Buckets[rightBucketIndex].ValueEnd);
                            break;
                        case JoinNode.ComparisonType.EqualOrMore:
                            inBounds = CheckEqualOrMoreBounds(
                                leftGram.Buckets[leftBucketIndex].ValueStart,
                                rightGram.Buckets[rightBucketIndex].ValueStart,
                                leftGram.Buckets[leftBucketIndex].ValueEnd,
                                rightGram.Buckets[rightBucketIndex].ValueEnd);
                            break;
                        case JoinNode.ComparisonType.None:
                            throw new ArgumentException("No join type specified : " + joinNode.ToString());
                        default:
                            throw new ArgumentException("Unhandled join type: " + joinNode.ToString());
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
