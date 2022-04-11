using Histograms.Caches;
using System.Data;
using System.Reflection;
using System.Text;

namespace Histograms.Models
{
    /// <summary>
    /// Contains n buckets, which each represent the same m number of elements.
    /// </summary>
    public class HistogramEquiDepthVariance : HistogramEquiDepth
    {
        public HistogramEquiDepthVariance(string tableName, string attributeName, int depth) : base(tableName, attributeName, depth)
        {
        }

        internal HistogramEquiDepthVariance(CachedHistogram histo) : base(histo.TableName, histo.AttributeName, histo.Depth)
        {
            foreach (var bucket in histo.Buckets)
            {
                Type? type = Type.GetType(bucket.ValueType);
                if (type == null)
                    throw new NullReferenceException("Unexpected null as cache type");

                if (type.GetInterface(nameof(IComparable)) != null)
                {
                    var valueStart = Convert.ChangeType(bucket.ValueStart, type) as IComparable;
                    var valueEnd = Convert.ChangeType(bucket.ValueEnd, type) as IComparable;

                    if (valueStart == null || valueEnd == null)
                        throw new ArgumentNullException("Read bucket value was invalid!");

                    Buckets.Add(new HistogramBucketVariance(valueStart, valueEnd, bucket.Count, bucket.Variance, bucket.Mean, bucket.StandardDeviation));
                }
            }
        }

        protected override void GenerateHistogramFromSorted(List<IComparable> sorted)
        {
            for (int bStart = 0; bStart < sorted.Count; bStart += Depth)
            {
                IComparable startValue = sorted[bStart];
                IComparable endValue = sorted[bStart];
                int countValue = 1;
                decimal mean = Convert.ToDecimal(endValue);
                decimal variance = 0;
                decimal standardDeviation = 0;

                for (int bIter = bStart + 1; bIter < bStart + Depth && bIter < sorted.Count; bIter++)
                {
                    countValue++;
                    mean += Convert.ToDecimal(sorted[bIter]);
                }
                mean = mean / countValue;
                for (int bIter = bStart; bIter < bStart + Depth && bIter < sorted.Count; bIter++)
                {
                    variance += (decimal)Math.Pow(Convert.ToDouble(sorted[bIter]) - (double)mean, 2);
                }
                if (countValue > 1 && variance != 0)
                    standardDeviation = (decimal)Math.Sqrt((double)variance / countValue - 1);
                else
                    variance = 0;
                if (variance < 0)
                    variance = 0;

                Buckets.Add(new HistogramBucketVariance(startValue, endValue, countValue, variance, mean, standardDeviation));
            }
        }

        public override object Clone()
        {
            var retObj = new HistogramEquiDepthVariance(TableName, AttributeName, Depth);
            foreach (var bucket in Buckets)
                if (bucket is IHistogramBucketVariance vari)
                    retObj.Buckets.Add(new HistogramBucketVariance(vari.ValueStart, vari.ValueEnd, vari.Count, vari.Variance, vari.Mean, vari.StandardDeviation));
            return retObj;
        }
    }
}