namespace Histograms.Models
{
    public class HistogramBucketVariance : HistogramBucket
    {
        public int Variance { get; set; }
        public int Mean { get; set; }

        public HistogramBucketVariance(IComparable valueStart, IComparable valueEnd, long count, int variance, int mean) : base(valueStart, valueEnd, count)
        {
            Variance = variance;
            Mean = mean;
        }

        public override string? ToString()
        {
            return $"Start: [{ValueStart}], End: [{ValueEnd}], Count: [{Count}]";
        }
    }
}
