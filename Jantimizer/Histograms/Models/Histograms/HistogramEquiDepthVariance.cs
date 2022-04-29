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

        public HistogramEquiDepthVariance(Guid histogramId, string tableName, string attributeName, int depth) : base(histogramId, tableName, attributeName, depth)
        {
        }

        private void GenerateHistogramFromSorted(List<IComparable> sorted)
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
            var retObj = new HistogramEquiDepthVariance(HistogramId, TableName, AttributeName, Depth);
            foreach (var bucket in Buckets)
                if (bucket.Clone() is IHistogramBucket acc)
                    retObj.Buckets.Add(acc);
            return retObj;
        }
    }
}