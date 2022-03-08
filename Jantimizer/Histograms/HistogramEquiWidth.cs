namespace Histograms
{
    public class HistogramEquiWidth : IHistogram
    {
        public IHistogramBucket[] Buckets { get; set; }

        public HistogramEquiWidth(int[] values, int bucketCount)
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