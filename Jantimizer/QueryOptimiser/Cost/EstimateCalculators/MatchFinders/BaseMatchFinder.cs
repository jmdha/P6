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

namespace QueryOptimiser.Cost.EstimateCalculators.MatchFinders
{
    public abstract class BaseMatchFinder<EstimatorType> : IMatchFinder<EstimatorType>
        where EstimatorType : INodeEstimate
    {
        internal enum MatchType
        {
            Undefined,
            Overlap, // If the match is via overlap
            Match, // If there is a match but there is no overlap
            None
        }

        internal EstimatorType Estimator { get; set; }

        public BaseMatchFinder(EstimatorType estimator)
        {
            Estimator = estimator;
        }

        internal MatchType DoesMatch(ComparisonType.Type comType, IComparable constant, IHistogramBucket bucket)
        {
            switch (comType)
            {
                case ComparisonType.Type.Less:
                    if (constant.IsLessThan(bucket.ValueStart))
                        return MatchType.Match;
                    else if (constant.IsLessThan(bucket.ValueEnd))
                        return MatchType.Overlap;
                    else
                        return MatchType.None;
                case ComparisonType.Type.EqualOrLess:
                    if (constant.IsLessThanOrEqual(bucket.ValueStart))
                        return MatchType.Match;
                    else if (constant.IsLessThanOrEqual(bucket.ValueEnd))
                        return MatchType.Overlap;
                    else
                        return MatchType.None;
                case ComparisonType.Type.More:
                    if (constant.IsLargerThan(bucket.ValueEnd))
                        return MatchType.Match;
                    else if (constant.IsLargerThan(bucket.ValueStart))
                        return MatchType.Overlap;
                    else
                        return MatchType.None;
                case ComparisonType.Type.EqualOrMore:
                    if (constant.IsLargerThanOrEqual(bucket.ValueEnd))
                        return MatchType.Match;
                    else if (constant.IsLargerThanOrEqual(bucket.ValueStart))
                        return MatchType.Overlap;
                    else
                        return MatchType.None;
                default:
                    return MatchType.Undefined;
            }
        }

        /// <summary>
        ///    Returns true if the constant is inside the bounds of the bucket
        /// </summary>
        /// <param name="constant"></param>
        /// <param name="bucket"></param>
        /// <returns></returns>
        internal bool DoesOverlap(IComparable constant, IHistogramBucket bucket)
        {
            if (bucket.ValueStart.IsLessThanOrEqual(constant) && bucket.ValueEnd.IsLargerThanOrEqual(constant))
                return true;
            return false;
        }

        internal bool DoesOverlap(IHistogramBucket leftBucket, IHistogramBucket rightBucket)
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
            // Left bucket is entirely within Right bucket
            // Right Bucket: |===========|
            // Left Bucket:     |=====|
            if (rightBucket.ValueStart.IsLessThanOrEqual(leftBucket.ValueStart) && rightBucket.ValueEnd.IsLargerThanOrEqual(leftBucket.ValueEnd))
                return true;
            return false;
        }
    }
}
