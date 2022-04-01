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
        protected override long CalculateCost(ComparisonType.Type predicate, IHistogramBucket[] leftBuckets, IHistogramBucket[] rightBuckets)
        {
            long leftSum = 0;
            long rightSum = 0;
            for (int i = 0; i < leftBuckets.Length; i++)
            {
                if (i == 0 && leftBuckets[0].ValueStart.CompareTo(rightBuckets[0].ValueStart) < 0)
                    leftSum += GetVariatedCount(predicate, leftBuckets[i] as HistogramBucketVariance, rightBuckets[0] as HistogramBucketVariance);
                else if (i == leftBuckets.Length - 1 && leftBuckets[i].ValueEnd.CompareTo(rightBuckets[rightBuckets.Length - 1].ValueEnd) > 0)
                    leftSum += GetVariatedCount(predicate, leftBuckets[i] as HistogramBucketVariance, rightBuckets[rightBuckets.Length - 1] as HistogramBucketVariance);
                else
                    leftSum += leftBuckets[i].Count;
            }
            for (int i = 0; i < rightBuckets.Length; i++)
            {
                if (i == 0 && rightBuckets[0].ValueStart.CompareTo(leftBuckets[0].ValueStart) < 0)
                    rightSum += GetVariatedCount(predicate, rightBuckets[i] as HistogramBucketVariance, leftBuckets[0] as HistogramBucketVariance);
                else if (i == rightBuckets.Length - 1 && rightBuckets[0].ValueEnd.CompareTo(leftBuckets[leftBuckets.Length - 1].ValueEnd) > 0)
                    rightSum += GetVariatedCount(predicate, rightBuckets[i] as HistogramBucketVariance, leftBuckets[leftBuckets.Length - 1] as HistogramBucketVariance);
                else
                    rightSum += rightBuckets[i].Count;
            }
            return leftSum * rightSum;
        }
        private double AsDouble(object value, out bool success)
        {
            if (value is decimal || value is float || value is double)
            {
                success = true;
                return Convert.ToDouble(value);
            }
            success = false;
            return 0;
        }

        private long AsLong(object value, out bool success)
        {
            if (value is sbyte || value is byte || value is short || value is ushort || value is int || value is uint || value is long || value is ulong)
            {
                success = true;
                return Convert.ToInt64(value);
            }
            success = false;
            return 0;
        }

        private long GetVariatedCount(ComparisonType.Type predicate, HistogramBucketVariance bucket, HistogramBucketVariance comparisonBucket)
        {
            // if all values in the bucket is the same
            if (bucket.Variance == 0)
                return bucket.Count;

            

            bool vsSuccess = false;
            bool veSuccess = false;
            bool cvsSuccess = false;
            bool cveSuccess = false;

            var vs = AsLong(bucket.ValueStart, out vsSuccess);
            var ve = AsLong(bucket.ValueEnd, out veSuccess);
            var cvs = AsLong(comparisonBucket.ValueStart, out cvsSuccess);
            var cve = AsLong(comparisonBucket.ValueStart, out cveSuccess);
            

            if (bucket.ValueStart is long vs && veSuccess && cvsSuccess && cveSuccess)
            {
                long startDiff = Math.Abs(cvs - vs);
                long endDiff = Math.Abs(cve - ve);
                long totalDiff = startDiff + endDiff;
                long valueRange = Math.Abs(ve - vs);

                double overlap = (double)totalDiff / valueRange;

                double overlapPercent = (double)overlap / valueRange;

                long overlappedCount = (long)(bucket.Count * overlapPercent);
                if (overlappedCount == 0)
                    return 1;
                else 
                    return overlappedCount;
            }

            return bucket.Count;
        }
    }
}
