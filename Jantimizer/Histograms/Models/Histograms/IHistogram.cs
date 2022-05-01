using System.Data;
using Tools.Models.JsonModels;

namespace Histograms.Models
{
    public interface IHistogram : ICloneable
    {
        public Guid HistogramId { get; }
        public List<TypeCode> AcceptedTypes { get; }
        public TypeCode DataTypeCode { get; }
        public TableAttribute TableAttribute { get; }
        public string TableName { get; }
        public string AttributeName { get; }
        public List<IHistogramBucket> Buckets { get; }
        public List<IHistogramSegmentationComparative> Segmentations { get; }
        public ulong RawDataSizeBytes { get; }

        public void GenerateHistogram(DataTable table, string key);
        public void GenerateHistogram(List<IComparable> column);
        public void GenerateHistogramFromSortedGroups(IEnumerable<ValueCount> sortedGroups);
        public void GenerateSegmentationsFromSortedGroups(IEnumerable<ValueCount> sortedGroups);
    }
}
