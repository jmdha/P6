using System.Data;
using System.Text;

namespace Histograms.Models
{
    public abstract class BaseHistogram : IHistogram, ICloneable
    {
        public Guid HistogramId { get; internal set; }
        public abstract List<TypeCode> AcceptedTypes { get; }
        public List<IHistogramBucket> Buckets { get; }
        public List<IHistogramSegmentation> Segmentations { get; }
        public string TableName { get; }
        public string AttributeName { get; }

        public BaseHistogram(string tableName, string attributeName) : this(Guid.NewGuid(), tableName, attributeName)
        {
        }

        public BaseHistogram(Guid histogramId, string tableName, string attributeName)
        {
            HistogramId = histogramId;
            TableName = tableName;
            AttributeName = attributeName;
            Buckets = new List<IHistogramBucket>();
            Segmentations = new List<IHistogramSegmentation>();
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
            foreach (var bucket in Buckets) {
                Segmentations.Add(new HistogramSegmentation()
                {
                    LowestValue = bucket.ValueStart,
                    ElementsBeforeNextSegmentation = bucket.Count
                });
            }

            // Remove any segmentations for the last unique value, to prevent duplicates (And there might be multiple from equidepth)
            ValueCount lastUniqueValue = sortedGroups.Last();
            foreach (var segmentation in Segmentations.Where(s => s.LowestValue == lastUniqueValue.Value))
                Segmentations.Remove(segmentation);

            // Add final segmentation
            Segmentations.Add(new HistogramSegmentation()
            {
                LowestValue = lastUniqueValue.Value,
                ElementsBeforeNextSegmentation = lastUniqueValue.Count
            });
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
