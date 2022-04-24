namespace Histograms.Models
{
    public interface IHistogramBucket : ICloneable
    {
        public Guid BucketId { get; }
        public IComparable ValueStart { get; }
        public IComparable ValueEnd { get; }
        public long Count { get; }
    }
}
