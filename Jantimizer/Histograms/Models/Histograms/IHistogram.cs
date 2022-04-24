using System.Data;

namespace Histograms.Models
{
    public interface IHistogram : ICloneable
    {
        public Guid HistogramId { get; }
        public List<TypeCode> AcceptedTypes { get; }
        public string TableName { get; }
        public string AttributeName { get; }
        public List<IHistogramBucket> Buckets { get; }

        public void GenerateHistogram(DataTable table, string key);
        public void GenerateHistogram(List<IComparable> column);
        public void GenerateHistogramFromSortedGroups(IEnumerable<ValueCount> sortedGroups);
    }
}
