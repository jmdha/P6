namespace Histograms
{
    public interface IHistogramBucket
    {
        public int ValueStart { get; }
        public int ValueEnd { get; }
        public int Count { get; }
    }
}
