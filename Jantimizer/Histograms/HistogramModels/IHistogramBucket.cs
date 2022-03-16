namespace Histograms
{
    public interface IHistogramBucket
    {
        public int ValueStart { get; }
        public int ValueEnd { get; set; }
        public int Count { get; set; }
    }
}
