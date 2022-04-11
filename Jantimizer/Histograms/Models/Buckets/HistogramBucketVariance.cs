namespace Histograms.Models
{
    public class HistogramBucketVariance : HistogramBucket, IHistogramBucketVariance
    {
        public decimal Variance { get; internal set; }
        public decimal Mean { get; internal set; }
        public decimal StandardDeviation { get; internal set; }

        public HistogramBucketVariance(IComparable valueStart, IComparable valueEnd, long count, decimal variance, decimal mean, decimal standardDeviation) : base(valueStart, valueEnd, count)
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
