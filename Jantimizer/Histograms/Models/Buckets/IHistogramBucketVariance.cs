namespace Histograms.Models
{
    public interface IHistogramBucketVariance : IHistogramBucket
    {
        public int Variance { get; }
        public int Mean { get; }
    }
}
