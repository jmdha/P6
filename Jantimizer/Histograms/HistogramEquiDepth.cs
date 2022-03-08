namespace Histograms
{
    /// <summary>
    /// Contains n buckets, which each represent the same m number of elements.
    /// </summary>
    public class HistogramEquiDepth : IHistogram
    {
        public IHistogramBucket[] Buckets { get; set; }

        public HistogramEquiDepth(int[] values, int bucketCount)
        {
            var sortedValues = values.OrderByDescending(x => -x).ToArray(); // Small first, large last

            Buckets = new HistogramBucket[bucketCount];
            int bucketSize = sortedValues.Length / bucketCount; // Rounding error

            for (int i = 0; i < bucketCount; i++)
            {
                Buckets[i] = new HistogramBucket()
                {
                    ValueStart = sortedValues[i * bucketSize],
                    Count = bucketSize
                };
            }

        }

    }
}