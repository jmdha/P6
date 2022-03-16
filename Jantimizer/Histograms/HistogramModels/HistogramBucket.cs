namespace Histograms
{
    public class HistogramBucket : IHistogramBucket
    {
        public int ValueStart { get; }
        private int _valueEnd;
        public int ValueEnd { get => _valueEnd; set {
                if (value < ValueStart)
                    throw new IndexOutOfRangeException("Bucket end value cannot be lower than start value!");
                _valueEnd = value;
            } }
        private int count;
        public int Count { get => count; set { 
                if (value < 0)
                    throw new IndexOutOfRangeException("Count for a bucket cannot be less than 0!");
                count = value;
            } }

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
