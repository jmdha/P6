namespace Histograms
{
    /// <summary>
    /// Contains n buckets, which each represent the same m number of elements.
    /// </summary>
    public class HistogramEquiDepth : IHistogram
    {
        public IHistogramBucket[] Buckets { get; }

        /// <summary>
        /// Creates Ceil(|values|/depth) buckets. The last bucket may not be full
        /// If called with 0 values, Buckets will be an empty array.
        /// </summary>
        /// <param name="values">The values to make a histogram for</param>
        /// <param name="depth">The max elements per bucket (All but last bucket will have max elements)</param>
        public HistogramEquiDepth(IEnumerable<int> values, int depth)
        {
            var sortedValues = values.OrderByDescending(x => -x).ToArray(); // Small first, large last

            int bucketCount = (int)Math.Ceiling((double)sortedValues.Length / depth);

            Buckets = new HistogramBucket[bucketCount];

            for (int i = 0; i < Buckets.Length; i++)
            {
                int remaining = sortedValues.Length - i * depth;
                Buckets[i] = new HistogramBucket(
                    sortedValues[i * depth],
                    Math.Min(depth, remaining)
                    );
            }
        }
    }
}