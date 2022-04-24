namespace Histograms.Models
{
    public class HistogramBucket : IHistogramBucket
    {
        public IComparable ValueStart { get; }
        private IComparable _valueEnd;
        public IComparable ValueEnd { get => _valueEnd; internal set {
                if (value.CompareTo(ValueStart) < 0)
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
        public Guid BucketId { get; internal set; }

        public HistogramBucket(IComparable valueStart, IComparable valueEnd, long count)
        {
            if(valueStart is null)
                throw new ArgumentNullException(nameof(valueStart));
            if(valueEnd is null)
                throw new ArgumentNullException(nameof(valueEnd));

            ValueStart = valueStart;

            _valueEnd = null!; // Suppress warning for _valueEnd "not being set"
            ValueEnd = valueEnd;
            Count = count;
            BucketId = Guid.NewGuid();
        }

        public HistogramBucket(Guid bucketId, IComparable valueStart, IComparable valueEnd, long count)
        {
            if (valueStart is null)
                throw new ArgumentNullException(nameof(valueStart));
            if (valueEnd is null)
                throw new ArgumentNullException(nameof(valueEnd));

            ValueStart = valueStart;

            _valueEnd = null!; // Suppress warning for _valueEnd "not being set"
            ValueEnd = valueEnd;
            Count = count;
            BucketId = bucketId;
        }

        public override string? ToString()
        {
            return $"Id: [{BucketId}], Type:[{this.GetType().Name}], Start: [{ValueStart}], End: [{ValueEnd}], Count: [{Count}]";
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ValueStart, ValueEnd, Count, BucketId);
        }

        public virtual object Clone()
        {
            return new HistogramBucket(BucketId, ValueStart, ValueEnd, Count);
        }
    }
}
