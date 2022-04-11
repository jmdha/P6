namespace Histograms.Models
{
    public interface IHistogramBucketVariance : IHistogramBucket
    {
        public decimal Variance { get; }
        public decimal StandardDeviation { get; }
        public decimal Mean { get; }
    }
}
