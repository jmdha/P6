namespace Histograms
{
    public class HistogramBucket : IHistogramBucket
    {
        public int ValueStart { get; }
        private int _valueEnd;
        public int ValueEnd { get => _valueEnd; internal set {
                if (value < ValueStart)
                    throw new IndexOutOfRangeException("Bucket end value cannot be lower than start value!");
                _valueEnd = value;
            } }
        private long count;
        public long Count { get => count; internal set
            { 
                if (value < 0)
                    throw new IndexOutOfRangeException("Count for a bucket cannot be less than 0!");
                count = value;
            } }

        public HistogramBucket(int valueStart, int valueEnd, long count)
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
