namespace Histograms
{
    public class HistogramBucket : IHistogramBucket
    {
        public int ValueStart { get; }
        public int Count { get; set; }

        public HistogramBucket(int valueStart, int count)
        {
            ValueStart = valueStart;
            Count = count;
        }

        public override string? ToString()
        {
            return $"Start: [{ValueStart}], Count: [{Count}]";
        }
    }
}
