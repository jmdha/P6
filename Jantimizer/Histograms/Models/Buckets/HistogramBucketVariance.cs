namespace Histograms.Models
{
    public class HistogramBucketVariance : HistogramBucket, IHistogramBucketVariance
    {
        public double Variance { get; internal set; }
        public double Mean { get; internal set; }
        public double StandardDeviation { get; internal set; }

        public HistogramBucketVariance(IComparable valueStart, IComparable valueEnd, long count, double variance, double mean, double standardDeviation) : base(valueStart, valueEnd, count)
        {
            Variance = variance;
            Mean = mean;
            StandardDeviation = standardDeviation;
        }

        public override string? ToString()
        {
            return $"Start: [{ValueStart}], End: [{ValueEnd}], Count: [{Count}], Variance: [{Variance}], Mean: [{Mean}], Standard Deviation [{StandardDeviation}]";
        }
    }
}
