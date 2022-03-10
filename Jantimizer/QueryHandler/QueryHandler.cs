using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QueryParser.Models;
using Histograms;
using DatabaseConnector;

namespace QueryHandler
{
    public class QueryHandler
    {
        public int CalculateJoinCost(JoinNode joinNode, Histograms.Managers.PostgresEquiDepthHistogramManager  histograms) {
            switch (joinNode.ComType) {
                case JoinNode.ComparisonType.Equal:
                    return CalculateEqualJoinCost(joinNode, histograms);
                case JoinNode.ComparisonType.EqualOrLess:
                    return CalculateEqualOrLessJoinCost(joinNode, histograms);
                case JoinNode.ComparisonType.EqualOrMore:
                    return CalculateEqualOrMoreJoinCost(joinNode, histograms);
                case JoinNode.ComparisonType.Less:
                    return CalculateLessJoinCost(joinNode, histograms);
                case JoinNode.ComparisonType.More:
                    return CalculateMoreJoinCost(joinNode, histograms);
                case JoinNode.ComparisonType.None:
                    throw new ArgumentException("No join type specified : " + joinNode.ToString());
                default:
                    throw new ArgumentException("Unhandled join type: " + joinNode.ToString());
            }
        }

        private int CalculateEqualJoinCost(JoinNode joinNode, Histograms.Managers.PostgresEquiDepthHistogramManager histograms) {
            IHistogram leftGram = histograms.GetHistogram(joinNode.LeftTable, joinNode.LeftAttribute);
            IHistogram rightGram = histograms.GetHistogram(joinNode.RightTable, joinNode.RightAttribute);
            int leftBucketStart = -1;
            int leftBucketEnd = -1;
            int rightBucketStart = -1;
            int rightBucketEnd = -1;
            
            for (int leftBucketIndex = 0; leftBucketIndex < leftGram.Buckets.Count; leftBucketIndex++) {
                for (int rightBucketIndex = 0; rightBucketIndex < rightGram.Buckets.Count; rightBucketIndex++) {
                    // If either rightBucketStart or end is in the range of the left bucket
                    if ((rightGram.Buckets[rightBucketIndex].ValueStart >= leftGram.Buckets[leftBucketIndex].ValueStart && 
                         rightGram.Buckets[rightBucketIndex].ValueStart <= leftGram.Buckets[leftBucketIndex].ValueEnd) ||
                        (rightGram.Buckets[rightBucketIndex].ValueEnd <= leftGram.Buckets[leftBucketIndex].ValueEnd &&
                         rightGram.Buckets[rightBucketIndex].ValueEnd >= leftGram.Buckets[leftBucketIndex].ValueStart)) {
                        if (leftBucketStart == -1)
                            leftBucketStart = leftBucketIndex;
                        if (leftBucketEnd < leftBucketIndex)
                            leftBucketEnd = leftBucketIndex;
                        if (rightBucketStart == -1)
                            rightBucketStart = rightBucketIndex;
                        if (rightBucketEnd < rightBucketIndex)
                            rightBucketEnd = rightBucketIndex;
                        continue;
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
        private int CalculateEqualOrLessJoinCost(JoinNode joinNode, Histograms.Managers.PostgresEquiDepthHistogramManager histograms) {
            throw new NotImplementedException();
        }
        private int CalculateEqualOrMoreJoinCost(JoinNode joinNode, Histograms.Managers.PostgresEquiDepthHistogramManager histograms) {
            throw new NotImplementedException();
        }
        private int CalculateLessJoinCost(JoinNode joinNode, Histograms.Managers.PostgresEquiDepthHistogramManager histograms) {
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
                    // If either rightBucketStart or end is in the range of the left bucket
                    if (leftGram.Buckets[leftBucketIndex].ValueStart < rightGram.Buckets[rightBucketIndex].ValueEnd)
                    {
                        if (leftBucketStart == -1)
                            leftBucketStart = leftBucketIndex;
                        if (leftBucketEnd < leftBucketIndex)
                            leftBucketEnd = leftBucketIndex;
                        if (rightBucketStart == -1)
                            rightBucketStart = rightBucketIndex;
                        if (rightBucketEnd < rightBucketIndex)
                            rightBucketEnd = rightBucketIndex;
                        continue;
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
        private int CalculateMoreJoinCost(JoinNode joinNode, Histograms.Managers.PostgresEquiDepthHistogramManager histograms) {
            throw new NotImplementedException();
        }
    }
}
