namespace Histograms
{
    public class HistogramBucket : IHistogramBucket
    {
        public int ValueStart { get; }
        public int Count { get; }

        public HistogramBucket(int valueStart, int count)
        {
            ValueStart = valueStart;
            Count = count;
        }
    }
}
