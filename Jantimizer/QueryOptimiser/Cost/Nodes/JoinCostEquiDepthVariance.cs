using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QueryParser.Models;
using Histograms;
using Histograms.Models;
using DatabaseConnector;

namespace QueryOptimiser.Cost.Nodes.EquiDepthVariance
{
    public class JoinCostEquiDepthVariance : BaseJoinCost
    {
        protected override long CalculateCost(ComparisonType.Type predicate, List<IHistogramBucket> leftBuckets, List<IHistogramBucket> rightBuckets)
        {
            long estimate = -1;

            switch (predicate)
            {
                case ComparisonType.Type.Equal:
                    return CalculateEqualCost(leftBuckets, rightBuckets);
                case ComparisonType.Type.Less:
                    return CalculateInEqualityCost(predicate, leftBuckets, rightBuckets);
                case ComparisonType.Type.More:
                    return CalculateInEqualityCost(predicate, leftBuckets, rightBuckets);
                case ComparisonType.Type.EqualOrLess:
                    return CalculateInEqualityCost(predicate, leftBuckets, rightBuckets);
                case ComparisonType.Type.EqualOrMore:
                    return CalculateInEqualityCost(predicate, leftBuckets, rightBuckets);
            }
            
            return estimate;
        }

        private long CalculateEqualCost(List<IHistogramBucket> leftBuckets, List<IHistogramBucket> rightBuckets)
        {
            long estimate = 0;
            int rightCutoff = 0;
            for (int i = 0; i < leftBuckets.Count; i++)
            {
                for (int j = rightCutoff; j < rightBuckets.Count; j++)
                {
                    if (DoesMatch(leftBuckets[i], rightBuckets[j]))
                        estimate += GetVariatedCount((HistogramBucketVariance)leftBuckets[i], (HistogramBucketVariance)rightBuckets[j]);
                    else
                        rightCutoff = Math.Max(0, j - 1);
                }
            }
            return estimate;
        }

        private long CalculateInEqualityCost(ComparisonType.Type predicateType, List<IHistogramBucket> leftBuckets, List<IHistogramBucket> rightBuckets)
        {
            long estimate = 0;
            int rightCutoff = rightBuckets.Count - 1;
            for (int i = leftBuckets.Count - 1; i >= 0; i--)
            {
                for (int j = rightCutoff; j >= 0; j--)
                {
                    bool Match = true;
                    switch(predicateType)
                    {
                        case ComparisonType.Type.Less:
                            if (leftBuckets[i].ValueEnd.CompareTo(rightBuckets[j].ValueStart) < 0)
                                estimate += GetCount((HistogramBucketVariance)leftBuckets[i], (HistogramBucketVariance)rightBuckets[j]);
                            else if (leftBuckets[i].ValueStart.CompareTo(rightBuckets[j].ValueEnd) < 0)
                                estimate += GetVariatedCount((HistogramBucketVariance)leftBuckets[i], (HistogramBucketVariance)rightBuckets[j]);
                            else 
                                Match = false;
                            break;
                        case ComparisonType.Type.EqualOrLess:
                            if (leftBuckets[i].ValueEnd.CompareTo(rightBuckets[j].ValueStart) <= 0)
                                estimate += GetCount((HistogramBucketVariance)leftBuckets[i], (HistogramBucketVariance)rightBuckets[j]);
                            else if (leftBuckets[i].ValueStart.CompareTo(rightBuckets[j].ValueEnd) <= 0)
                                estimate += GetVariatedCount((HistogramBucketVariance)leftBuckets[i], (HistogramBucketVariance)rightBuckets[j]);
                            else
                                Match = false;
                            break;
                        case ComparisonType.Type.More:
                            if (leftBuckets[i].ValueStart.CompareTo(rightBuckets[j].ValueEnd) > 0)
                                estimate += GetCount((HistogramBucketVariance)leftBuckets[i], (HistogramBucketVariance)rightBuckets[j]);
                            else if (leftBuckets[i].ValueEnd.CompareTo(rightBuckets[j].ValueStart) > 0)
                                estimate += GetVariatedCount((HistogramBucketVariance)leftBuckets[i], (HistogramBucketVariance)rightBuckets[j]);
                            else
                                Match = false;
                            break;
                        case ComparisonType.Type.EqualOrMore:
                            if (leftBuckets[i].ValueStart.CompareTo(rightBuckets[j].ValueEnd) >= 0)
                                estimate += GetCount((HistogramBucketVariance)leftBuckets[i], (HistogramBucketVariance)rightBuckets[j]);
                            else if (leftBuckets[i].ValueEnd.CompareTo(rightBuckets[j].ValueStart) >= 0)
                                estimate += GetVariatedCount((HistogramBucketVariance)leftBuckets[i], (HistogramBucketVariance)rightBuckets[j]);
                            else
                                Match = false;
                            break;
                    }

                    if (Match)
                        rightCutoff = Math.Max(0, j - 1);
                }
            }
            return estimate;
        }

        private bool DoesMatch(IHistogramBucket leftBucket, IHistogramBucket rightBucket)
        {
            if ((rightBucket.ValueStart.CompareTo(leftBucket.ValueStart) >= 0 && rightBucket.ValueStart.CompareTo(leftBucket.ValueEnd) <= 0) ||
                (rightBucket.ValueEnd.CompareTo(leftBucket.ValueStart) >= 0 && rightBucket.ValueEnd.CompareTo(leftBucket.ValueEnd) <= 0))
                return true;
            return false;
        }

        private long GetCount(HistogramBucketVariance bucket, HistogramBucketVariance comparisonBucket)
        {
            return bucket.Count * comparisonBucket.Count;
        }

        private long GetVariatedCount(HistogramBucketVariance bucket, HistogramBucketVariance comparisonBucket)
        {
            double certainty = (double)Math.Abs((double)bucket.Variance / comparisonBucket.Variance);
            if (certainty > 1)
                certainty = 1 / certainty;
            long estimate = (long)(bucket.Count * certainty);
            if (estimate == 0)
                return 1;
            else
                return estimate;
        }
    }
}
