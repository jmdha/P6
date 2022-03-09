namespace Histograms
{
    public interface IHistogram
    {
        public IHistogramBucket[] Buckets { get; }
    }
}
