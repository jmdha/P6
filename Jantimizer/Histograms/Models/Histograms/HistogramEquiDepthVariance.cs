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
        public override List<TypeCode> AcceptedTypes { get; } = new List<TypeCode>() { 
            TypeCode.Int16, 
            TypeCode.Int32, 
            TypeCode.Int64,
            TypeCode.Double
        };

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

                    Buckets.Add(new HistogramBucketVariance(valueStart, valueEnd, bucket.Count, bucket.Variance, bucket.Mean, bucket.StandardDeviation, Convert.ToDouble(valueEnd) - Convert.ToDouble(valueStart)));
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
                double mean = Convert.ToDouble(endValue);
                double variance = 0;
                double standardDeviation = 0;

                for (int bIter = bStart + 1; bIter < bStart + Depth && bIter < sorted.Count; bIter++)
                {
                    countValue++;
                    endValue = sorted[bIter];
                    mean += Convert.ToDouble(endValue);
                }
                mean = mean / countValue;
                for (int bIter = bStart; bIter < bStart + Depth && bIter < sorted.Count; bIter++)
                {
                    variance += Math.Pow(Convert.ToDouble(sorted[bIter]) - mean, 2);
                }
                if (countValue > 1 && variance != 0)
                    standardDeviation = Math.Sqrt(variance / countValue);
                else
                    variance = 0;
                if (variance < 0)
                    variance = 0;

                Buckets.Add(new HistogramBucketVariance(startValue, endValue, countValue, variance, mean, standardDeviation, Convert.ToDouble(endValue) - Convert.ToDouble(startValue)));
            }
        }

        public override object Clone()
        {
            var retObj = new HistogramEquiDepthVariance(TableName, AttributeName, Depth);
            foreach (var bucket in Buckets)
                if (bucket is IHistogramBucketVariance vari)
                    retObj.Buckets.Add(new HistogramBucketVariance(vari.ValueStart, vari.ValueEnd, vari.Count, vari.Variance, vari.Mean, vari.StandardDeviation, Convert.ToDouble(vari.ValueEnd) - Convert.ToDouble(vari.ValueStart)));
            return retObj;
        }

        public override bool Equals(object? obj)
        {
            return obj is HistogramEquiDepthVariance variance &&
                   base.Equals(obj) &&
                   EqualityComparer<List<TypeCode>>.Default.Equals(AcceptedTypes, variance.AcceptedTypes) &&
                   EqualityComparer<List<IHistogramBucket>>.Default.Equals(Buckets, variance.Buckets) &&
                   TableName == variance.TableName &&
                   AttributeName == variance.AttributeName &&
                   EqualityComparer<List<TypeCode>>.Default.Equals(AcceptedTypes, variance.AcceptedTypes) &&
                   Depth == variance.Depth &&
                   EqualityComparer<List<TypeCode>>.Default.Equals(AcceptedTypes, variance.AcceptedTypes);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(base.GetHashCode(), AcceptedTypes, Buckets, TableName, AttributeName, AcceptedTypes, Depth, AcceptedTypes);
        }
    }
}