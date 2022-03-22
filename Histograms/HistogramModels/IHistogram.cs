using System.Data;

namespace Histograms
{
    public interface IHistogram
    {
        public string TableName { get; }
        public string AttributeName { get; }
        public List<IHistogramBucket> Buckets { get; }

        public void GenerateHistogram(DataTable table, string key);
        public void GenerateHistogram(List<int> column);
    }
}
