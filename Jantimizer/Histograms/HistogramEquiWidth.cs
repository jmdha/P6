namespace Histograms
{
    /// <summary>
    /// Contains n buckets, which each represent a certain range of value
    /// </summary>
    public class HistogramEquiWidth : IHistogram
    {
        public IHistogramBucket[] Buckets { get; set; }

        public HistogramEquiWidth(int[] values, int bucketCount)
        {
            throw new NotImplementedException();
        }

    }
}