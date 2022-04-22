﻿using Histograms.Models;
using QueryOptimiser.Cost.Nodes;
using QueryOptimiser.Models;
using QueryParser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.Helpers;

namespace QueryOptimiser.Cost.EstimateCalculators.MatchFinders
{
    public class JoinMatchFinder : BaseMatchFinder<IJoinEstimate>
    {
        public JoinMatchFinder(IJoinEstimate estimator) : base(estimator)
        {
        }

        internal List<IntermediateBucket> GetMatches(JoinPredicate predicate, List<IHistogramBucket> leftBuckets, List<IHistogramBucket> rightBuckets)
        {
            switch (predicate.ComType)
            {
                case ComparisonType.Type.Equal:
                    return GetEqualityMatches(predicate, leftBuckets, rightBuckets);
                case ComparisonType.Type.Less:
                case ComparisonType.Type.More:
                case ComparisonType.Type.EqualOrMore:
                case ComparisonType.Type.EqualOrLess:
                    return GetInEqualityMatches(predicate, leftBuckets, rightBuckets);
                default:
                    throw new ArgumentException($"Invalid comparison type {predicate.ToString()}");
            }
        }

        internal List<IntermediateBucket> GetEqualityMatches(JoinPredicate predicate, List<IHistogramBucket> leftBuckets, List<IHistogramBucket> rightBuckets)
        {
            List<IntermediateBucket> buckets = new List<IntermediateBucket>();
            int rightCutoff = 0;
            for (int i = 0; i < leftBuckets.Count; i++)
            {
                for (int j = rightCutoff; j < rightBuckets.Count; j++)
                {
                    if (DoesOverlap(leftBuckets[i], rightBuckets[j]))
                        buckets.Add(MakeNewIntermediateBucket(MatchType.Overlap, predicate, leftBuckets[i], rightBuckets[j]));
                    else
                    {
                        rightCutoff = Math.Max(0, j - 1);
                        break;
                    }
                }
            }
            return buckets;
        }

        /// <summary>
        /// Makes a list of intermediate buckets, based on left and right histogram buckets.
        /// Example
        /// <code>
        ///     bucket = b(1,10)  from value 1 to 10
        ///     leftBuckets = { b(1,10), b(11,20), b(21, 30) }
        ///     rightBuckets = { b(1,15), b(16,40) }
        ///     Predicate = Larger than    
        /// 
        ///     Then we check start value against end values, and vise versa
        ///     
        ///     So for each of the left buckets:
        ///         b(1,10):
        ///             for each of the right buckets:
        ///                 is 10(left bucket end) larger than 1(right bucket start)? or is 1(left bucket start) larger than 15(right bucket end)? YES, add new intermediate bucket.
        ///                 is 10 larger than 16 or is 1 larger than 40? No
        ///                     we can then set an offset ´rightCutoff´ to be index 1, and break.
        ///         b(11,20)
        ///             for each of the right buckets (from index 1 and up):
        ///                 is 20 larger than 16 or is 11 larger than 40? YES, add new intermediate bucket.
        ///         b(21,30)
        ///             for each of the right buckets (from index 1 and up):
        ///                 is 30 larger than 16 or is 21 larger than 40? YES, add new intermediate bucket.
        ///     
        ///     This then gives us 3 new intermediate buckets.
        /// </code>
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="leftBuckets"></param>
        /// <param name="rightBuckets"></param>
        /// <returns></returns>
        internal List<IntermediateBucket> GetInEqualityMatches(JoinPredicate predicate, List<IHistogramBucket> leftBuckets, List<IHistogramBucket> rightBuckets)
        {
            List<IntermediateBucket> buckets = new List<IntermediateBucket>();
            int rightCutoff = 0;
            for (int i = 0; i < leftBuckets.Count; i++)
            {
                for (int j = rightCutoff; j < rightBuckets.Count; j++)
                {
                    MatchType matchType = MatchType.Undefined;
                    switch (predicate.ComType)
                    {
                        case ComparisonType.Type.Less:
                            if (leftBuckets[i].ValueEnd.IsLessThan(rightBuckets[j].ValueStart))
                                matchType = MatchType.Match;
                            else if (leftBuckets[i].ValueStart.IsLessThan(rightBuckets[j].ValueEnd))
                                matchType = MatchType.Overlap;
                            else
                                matchType = MatchType.None;
                            break;
                        case ComparisonType.Type.EqualOrLess:
                            if (leftBuckets[i].ValueEnd.IsLessThanOrEqual(rightBuckets[j].ValueStart))
                                matchType = MatchType.Match;
                            else if (leftBuckets[i].ValueStart.IsLessThanOrEqual(rightBuckets[j].ValueEnd))
                                matchType = MatchType.Overlap;
                            else
                                matchType = MatchType.None;
                            break;
                        case ComparisonType.Type.More:
                            if (leftBuckets[i].ValueStart.IsLargerThan(rightBuckets[j].ValueEnd))
                                matchType = MatchType.Match;
                            else if (leftBuckets[i].ValueEnd.IsLargerThan(rightBuckets[j].ValueStart))
                                matchType = MatchType.Overlap;
                            else
                                matchType = MatchType.None;
                            break;
                        case ComparisonType.Type.EqualOrMore:
                            if (leftBuckets[i].ValueStart.IsLargerThanOrEqual(rightBuckets[j].ValueEnd))
                                matchType = MatchType.Match;
                            else if (leftBuckets[i].ValueEnd.IsLargerThanOrEqual(rightBuckets[j].ValueStart))
                                matchType = MatchType.Overlap;
                            else
                                matchType = MatchType.None;
                            break;
                    }
                    if (matchType == MatchType.Match || matchType == MatchType.Overlap)
                        buckets.Add(MakeNewIntermediateBucket(matchType, predicate, leftBuckets[i], rightBuckets[j]));
                    else
                    {
                        rightCutoff = Math.Max(0, j - 1);
                        break;
                    }
                }
            }
            return buckets;
        }

        internal IntermediateBucket MakeNewIntermediateBucket(MatchType matchType, JoinPredicate predicate, IHistogramBucket leftBucket, IHistogramBucket rightBucket)
        {
            var newBucket = new IntermediateBucket();
            long leftCount;
            long rightCount;
            if (matchType == MatchType.Match)
            {
                leftCount = leftBucket.Count;
                rightCount = rightBucket.Count;
            }
            else if (matchType == MatchType.Overlap)
            {
                leftCount = Estimator.GetBucketEstimate(predicate.ComType, leftBucket, rightBucket);
                rightCount = Estimator.GetBucketEstimate(predicate.ComType, rightBucket, leftBucket);
            }
            else
                throw new ArgumentException($"Invalid matchtype {matchType}");

            newBucket.AddBucketIfNotThere(
                new TableAttribute(predicate.LeftTable.TableName, predicate.LeftAttribute),
                new BucketEstimate(
                    leftBucket,
                    leftCount
                    )
                );
            newBucket.AddBucketIfNotThere(
                new TableAttribute(predicate.RightTable.TableName, predicate.RightAttribute),
                new BucketEstimate(
                    rightBucket,
                    rightCount
                    )
                );
            return newBucket;
        }
    }
}