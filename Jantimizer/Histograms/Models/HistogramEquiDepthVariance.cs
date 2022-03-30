using System.Data;
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

        protected override void GenerateHistogramFromSorted(List<IComparable> sorted)
        {
            for (int bStart = 0; bStart < sorted.Count; bStart += Depth)
            {
                IComparable startValue = sorted[bStart];
                IComparable endValue = sorted[bStart];
                int countValue = 1;
                int mean = (int)endValue;
                int variance = 0;

                for (int bIter = bStart + 1; bIter < bStart + Depth && bIter < sorted.Count; bIter++)
                {
                    countValue++;
                    endValue = sorted[bIter];
                    mean += (int)endValue;
                }
                mean = mean / countValue;
                for (int bIter = bStart; bIter < bStart + Depth && bIter < sorted.Count; bIter++)
                {
                    variance += (int)Math.Pow((int)sorted[bIter] - mean, 2);
                }
                variance = (int)Math.Sqrt(variance / countValue - 1);

                Buckets.Add(new HistogramBucketVariance(startValue, endValue, countValue, variance, mean));
            }
        }
    }
}