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

        #region DoesMatch
        internal MatchType DoesMatch(ComparisonType.Type comType, IComparable constant, IHistogramBucket bucket)
        {
            return DoesMatch(comType, constant, bucket.ValueStart, bucket.ValueEnd);
        }

        internal MatchType DoesMatch(ComparisonType.Type comType, IHistogramBucket leftBucket, IHistogramBucket rightBucket)
        {
            return DoesMatch(comType, leftBucket.ValueStart, leftBucket.ValueEnd, rightBucket.ValueStart, rightBucket.ValueEnd);
        }

        internal MatchType DoesMatch(ComparisonType.Type comType, IComparable range1Start, IComparable range1End, IComparable range2Start, IComparable range2End)
        {
            MatchType startMatch = DoesMatch(comType, range1Start, range2Start, range2End);
            MatchType endMatch = DoesMatch(comType, range1End, range2Start, range2End);

            if (startMatch == MatchType.Match && endMatch == MatchType.Match)
                return MatchType.Match;
            else if (startMatch == MatchType.Overlap || endMatch == MatchType.Overlap)
                return MatchType.Overlap;
            else if (startMatch == MatchType.Match || endMatch == MatchType.Match)
                return MatchType.Overlap;
            else
                return MatchType.None;
        }

        internal MatchType DoesMatch(ComparisonType.Type comType, IComparable constant, IComparable rangeStart, IComparable rangeEnd)
        {
            switch (comType)
            {
                case ComparisonType.Type.Less:
                    if (constant.IsLessThan(rangeEnd))
                        return MatchType.Match;
                    else if (constant.IsLessThan(rangeEnd))
                        return MatchType.Overlap;
                    else
                        return MatchType.None;
                case ComparisonType.Type.EqualOrLess:
                    if (constant.IsLessThanOrEqual(rangeStart))
                        return MatchType.Match;
                    else if (constant.IsLessThanOrEqual(rangeEnd))
                        return MatchType.Overlap;
                    else
                        return MatchType.None;
                case ComparisonType.Type.More:
                    if (constant.IsLargerThan(rangeEnd))
                        return MatchType.Match;
                    else if (constant.IsLargerThan(rangeStart))
                        return MatchType.Overlap;
                    else
                        return MatchType.None;
                case ComparisonType.Type.EqualOrMore:
                    if (constant.IsLargerThanOrEqual(rangeEnd))
                        return MatchType.Match;
                    else if (constant.IsLargerThanOrEqual(rangeStart))
                        return MatchType.Overlap;
                    else
                        return MatchType.None;
                default:
                    return MatchType.Undefined;
            }
        }
        #endregion
        #region DoesOverlap
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
        #endregion

        protected IntermediateBucket MakeNewIntermediateBucket(List<TableAttribute> tableAttributes, List<BucketEstimate> bucketEstimates)
        {
            if (tableAttributes.Count != bucketEstimates.Count)
                throw new ArgumentException("Unbalanced intermediate bucket creation");

            var newBucket = new IntermediateBucket();

            for (int i = 0; i < tableAttributes.Count; i++)
                newBucket.AddBucketIfNotThere(tableAttributes[i], bucketEstimates[i]);

            return newBucket;
        }
    }
}
