using Histograms.Models;
using QueryOptimiser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.Helpers;
using Tools.Models.JsonModels;

namespace QueryOptimiser.Cost.EstimateCalculators
{
    internal static class BoundsFinder
    {
        /// <summary>
        /// To get new lists from left and right buckets depending on predicate
        /// <code>
        ///    Bucket ranges example (For "MoreOrEqual" Predicate):
        ///        |==|  is a bucket
        ///     
        ///    v leftStart        v leftEnd
        ///    |==========||======|                      = leftBuckets
        ///    
        ///    v rightStart           v rightEnd
        ///    |==========||==========||==============|  = rightBuckets
        /// </code>
        /// </summary>
        /// <param name="predicateType"></param>
        /// <param name="leftBuckets"></param>
        /// <param name="rightBuckets"></param>
        /// <returns></returns>
        internal static PairBucketList GetBucketBounds(ComparisonType.Type predicateType, List<IHistogramBucket> leftBuckets, List<IHistogramBucket> rightBuckets)
        {
            int leftStart = 0;
            int leftEnd = leftBuckets.Count - 1;
            int rightStart = 0;
            int rightEnd = rightBuckets.Count - 1;

            if (leftBuckets.Count == 0 || rightBuckets.Count == 0)
                return new PairBucketList();

            // First bucket
            // Left:    |===|
            // Right:     |===|
            // OR
            // Left:    |=======|
            // Right:     |===|
            if (leftBuckets[0].ValueStart.IsLessThan(rightBuckets[0].ValueStart))
            {
                if (predicateType != ComparisonType.Type.Less && predicateType != ComparisonType.Type.EqualOrLess)
                    leftStart = GetMatchBucketIndex(leftBuckets, 0, leftBuckets.Count - 1, rightBuckets[0].ValueStart);
            }
            // First bucket
            // Left:      |===|
            // Right:   |===|
            // OR
            // Left:      |===|
            // Right:   |=======|
            else if (leftBuckets[0].ValueStart.IsLargerThan(rightBuckets[0].ValueStart))
            {
                if (predicateType != ComparisonType.Type.More && predicateType != ComparisonType.Type.EqualOrMore)
                    rightStart = GetMatchBucketIndex(rightBuckets, 0, rightBuckets.Count - 1, leftBuckets[0].ValueStart);
            }

            // Last bucket
            // Left:      |===|
            // Right:   |===|
            if (leftBuckets[leftBuckets.Count - 1].ValueEnd.IsLargerThan(rightBuckets[rightBuckets.Count - 1].ValueEnd))
            {
                if (predicateType != ComparisonType.Type.More && predicateType != ComparisonType.Type.EqualOrMore)
                    leftEnd = GetMatchBucketIndex(leftBuckets, 0, leftBuckets.Count - 1, rightBuckets[rightBuckets.Count - 1].ValueEnd);
            }
            // Last bucket
            // Left:    |===|
            // Right:     |===|
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

        /// <summary>
        /// Binary Search
        /// </summary>
        /// <param name="buckets"></param>
        /// <param name="lowerIndexBound"></param>
        /// <param name="upperIndexBound"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        internal static int GetMatchBucketIndex(List<IHistogramBucket> buckets, int lowerIndexBound, int upperIndexBound, IComparable value)
        {
            if (upperIndexBound >= lowerIndexBound)
            {
                int mid = lowerIndexBound + (upperIndexBound - lowerIndexBound) / 2;
                if (mid >= buckets.Count)
                    return -1;

                if ((value.IsLargerThanOrEqual(buckets[mid].ValueStart)) &&
                    (value.IsLessThanOrEqual(buckets[mid].ValueEnd)))
                    return mid;

                if (value.IsLessThan(buckets[mid].ValueEnd))
                    return GetMatchBucketIndex(buckets, lowerIndexBound, mid - 1, value);

                return GetMatchBucketIndex(buckets, mid + 1, upperIndexBound, value);
            }

            return -1;
        }
    }
}
