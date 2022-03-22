namespace Histograms
{
    public interface IHistogramBucket
    {
        public IComparable ValueStart { get; }
        public IComparable ValueEnd { get; }
        public long Count { get; }
    }
}
