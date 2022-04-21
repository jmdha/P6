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

        public override string? ToString()
        {
            return $"Start: [{ValueStart}], End: [{ValueEnd}], Count: [{Count}], Variance: [{Variance}], Mean: [{Mean}], Standard Deviation [{StandardDeviation}]";
        }

        public override bool Equals(object? obj)
        {
            return obj is HistogramBucketVariance variance &&
                   base.Equals(obj) &&
                   EqualityComparer<IComparable>.Default.Equals(ValueStart, variance.ValueStart) &&
                   EqualityComparer<IComparable>.Default.Equals(ValueEnd, variance.ValueEnd) &&
                   Count == variance.Count &&
                   BucketId.Equals(variance.BucketId) &&
                   Variance == variance.Variance &&
                   Mean == variance.Mean &&
                   StandardDeviation == variance.StandardDeviation &&
                   Range == variance.Range;
        }

        public override int GetHashCode()
        {
            HashCode hash = new HashCode();
            hash.Add(base.GetHashCode());
            hash.Add(ValueStart);
            hash.Add(ValueEnd);
            hash.Add(Count);
            hash.Add(BucketId);
            hash.Add(Variance);
            hash.Add(Mean);
            hash.Add(StandardDeviation);
            hash.Add(Range);
            return hash.ToHashCode();
        }
    }
}
