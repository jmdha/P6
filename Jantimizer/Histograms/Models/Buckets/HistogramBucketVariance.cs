namespace Histograms.Models
{
    public class HistogramBucketVariance : HistogramBucket, IHistogramBucketVariance
    {
        public int Variance { get; internal set; }
        public int Mean { get; internal set; }
        public int Range { get; internal set; }

        public HistogramBucketVariance(IComparable valueStart, IComparable valueEnd, long count, int variance, int mean, int range) : base(valueStart, valueEnd, count)
        {
            Variance = variance;
            Mean = mean;
            Range = range;
        }

        public override string? ToString()
        {
            return $"Start: [{ValueStart}], End: [{ValueEnd}], Count: [{Count}], Variance: [{Variance}], Mean: [{Mean}]";
        }
    }
}
