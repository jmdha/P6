namespace Histograms
{
    public class HistogramBucket : IHistogramBucket
    {
        public int ValueStart { get; }
        public int ValueEnd { get; set; }
        public int Count { get; set; }

        public HistogramBucket(int valueStart, int valueEnd, int count)
        {
            ValueStart = valueStart;
            ValueEnd = valueEnd;
            Count = count;
        }

        public override string? ToString()
        {
            return $"Start: [{ValueStart}], End: [{ValueEnd}], Count: [{Count}]";
        }
    }
}
