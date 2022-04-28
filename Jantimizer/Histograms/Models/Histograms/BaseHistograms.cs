using System.Data;
using System.Text;
using Tools.Models.JsonModels;

namespace Histograms.Models
{
    public abstract class BaseHistogram : IHistogram, ICloneable
    {
        public Guid HistogramId { get; internal set; }
        public abstract List<TypeCode> AcceptedTypes { get; }
        public TypeCode DataTypeCode { get; internal set; }
        public List<IHistogramBucket> Buckets { get; }
        public List<IHistogramSegmentationComparative> Segmentations { get; }
        public TableAttribute TableAttribute { get; }
        public string TableName => TableAttribute.Table.TableName;
        public string AttributeName => TableAttribute.Attribute;

        public BaseHistogram(string tableName, string attributeName) : this(Guid.NewGuid(), tableName, attributeName)
        { }

        public BaseHistogram(Guid histogramId, string tableName, string attributeName) : this(histogramId, new TableAttribute(tableName, attributeName))
        { }
        public BaseHistogram(Guid histogramId, TableAttribute tableAttribute)
        {
            HistogramId = histogramId;
            TableAttribute = tableAttribute;
            Buckets = new List<IHistogramBucket>();
            Segmentations = new List<IHistogramSegmentationComparative>();
        }

        public void GenerateHistogram(DataTable table, string key)
        {
            List<IComparable> sorted = table.AsEnumerable().Select(x => (IComparable)x[key]).OrderBy(x => x).ToList();
            GenerateHistogramFromSorted(sorted);
        }

        public void GenerateHistogram(List<IComparable> column)
        {
            List<IComparable> sorted = column.OrderBy(x => x).ToList();
            GenerateHistogramFromSorted(sorted);
        }

        private void GenerateHistogramFromSorted(List<IComparable> sorted)
        {
            GenerateHistogramFromSortedGroups(sorted
                .GroupBy(x => x)
                .Select(group =>
                    new ValueCount(
                        group.First(),
                        group.Count()
                    )
                )
            );
        }

        public abstract void GenerateHistogramFromSortedGroups(IEnumerable<ValueCount> sortedGroups);
        public void GenerateSegmentationsFromSortedGroups(IEnumerable<ValueCount> sortedGroups)
        {
            GenerateHistogramFromSortedGroups(sortedGroups);

            // Turn all bucket-starts into segmentations
            foreach (var bucket in Buckets)
            {
                Segmentations.Add(new HistogramSegmentationComparative(bucket.ValueStart, bucket.Count));
            }

            // Remove any segmentations for the last unique value, to prevent duplicates (And there might be multiple from equidepth)
            ValueCount lastUniqueValue = sortedGroups.Last();
            foreach (var segmentation in Segmentations.Where(s => s.LowestValue == lastUniqueValue.Value).ToList())
                Segmentations.Remove(segmentation);

            // Add final segmentation
            Segmentations.Add(new HistogramSegmentationComparative(lastUniqueValue.Value, lastUniqueValue.Count));
            DataTypeCode = Type.GetTypeCode(Segmentations.First().LowestValue.GetType());
        }

        public override string? ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"\t Histogram ID {HistogramId}");
            sb.AppendLine($"\t Type {this.GetType().Name}");
            sb.AppendLine($"\t Data for attribute {TableName}.{AttributeName}:");
            foreach (var bucket in Buckets)
            {
                sb.AppendLine($"\t\t{bucket}");
            }
            return sb.ToString();
        }

        public abstract object Clone();

        public override int GetHashCode()
        {
            int hash = 0;
            foreach (var type in AcceptedTypes)
                hash += type.GetHashCode();
            foreach (var bucket in Buckets)
                hash += bucket.GetHashCode();
            return hash + HashCode.Combine(TableName, AttributeName, HistogramId);
        }
    }
}
