namespace Histograms.Models
{
    public interface IHistogramBucketVariance : IHistogramBucket
    {
        public double Variance { get; }
        public double StandardDeviation { get; }
        public double Mean { get; }
    }
}
