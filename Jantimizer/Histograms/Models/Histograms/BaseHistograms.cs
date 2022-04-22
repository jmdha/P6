using System.Data;
using System.Text;

namespace Histograms.Models
{
    public abstract class BaseHistogram : IHistogram, ICloneable
    {
        public Guid HistogramId { get; internal set; }
        public abstract List<TypeCode> AcceptedTypes { get; }
        public List<IHistogramBucket> Buckets { get; }
        public string TableName { get; }
        public string AttributeName { get; }

        public BaseHistogram(string tableName, string attributeName)
        {
            TableName = tableName;
            AttributeName = attributeName;
            Buckets = new List<IHistogramBucket>();
        }

        public BaseHistogram(Guid histogramId, string tableName, string attributeName)
        {
            HistogramId = histogramId;
            TableName = tableName;
            AttributeName = attributeName;
            Buckets = new List<IHistogramBucket>();
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

        protected abstract void GenerateHistogramFromSorted(List<IComparable> sorted);

        public abstract void GenerateHistogramFromSortedGroups(IEnumerable<ValueCount> sortedGroups);

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
