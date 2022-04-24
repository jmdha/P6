namespace Histograms.Models
{
    public class HistogramBucketVariance : HistogramBucket, IHistogramBucketVariance
    {
        public double Variance { get; internal set; }
        public double Mean { get; internal set; }
        public double StandardDeviation { get; internal set; }
        public double Range { get; internal set; }

        public HistogramBucketVariance(IComparable valueStart, IComparable valueEnd, long count, double variance, double mean, double standardDeviation, double range) : base(valueStart, valueEnd, count)
        {
            Variance = variance;
            Mean = mean;
            StandardDeviation = standardDeviation;
            Range = range;
        }

        public HistogramBucketVariance(Guid bucketId, IComparable valueStart, IComparable valueEnd, long count, double variance, double mean, double standardDeviation, double range) : base(bucketId, valueStart, valueEnd, count)
        {
            Variance = variance;
            Mean = mean;
            StandardDeviation = standardDeviation;
            Range = range;
        }

        public override string? ToString()
        {
            return $"Start: [{ValueStart}], End: [{ValueEnd}], Count: [{Count}], Variance: [{Variance}], Mean: [{Mean}], Standard Deviation [{StandardDeviation}]";
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() + HashCode.Combine(Variance, Mean, StandardDeviation, Range);
        }

        public override object Clone()
        {
            return new HistogramBucketVariance(BucketId, ValueStart, ValueEnd, Count, Variance, Mean, StandardDeviation, Range);
        }
    }
}
