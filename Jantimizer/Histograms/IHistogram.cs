using System.Data;

namespace Histograms
{
    public interface IHistogram
    {
        public List<IHistogramBucket> Buckets { get; }

        public void GenerateHistogram(DataTable table, string key);
        public void GenerateHistogram(List<int> column);
    }
}
