using System.Data;
using System.Text;

namespace Histograms
{
    /// <summary>
    /// Contains n buckets, which each represent the same m number of elements.
    /// </summary>
    public class HistogramEquiDepth : IHistogram
    {
        public List<IHistogramBucket> Buckets { get; }
        public string TableName { get; }
        public string AttributeName { get; }

        public int Depth { get; }

        public HistogramEquiDepth(string tableName, string attributeName, int depth)
        {
            TableName = tableName;
            AttributeName = attributeName;
            Depth = depth;
            Buckets = new List<IHistogramBucket>();
        }

        public void GenerateHistogram(DataTable table, string key)
        {
            List<int> sorted = table.AsEnumerable().Select(x => (int)x[key]).OrderByDescending(x => -x).ToList();
            GenerateHistogramFromSorted(sorted);
        }

        public void GenerateHistogram(List<int> column)
        {
            List<int> sorted = column.OrderByDescending(x => -x).ToList();
            GenerateHistogramFromSorted(sorted);
        }

        private void GenerateHistogramFromSorted(List<int> sorted)
        {
            for (int bStart = 0; bStart < sorted.Count; bStart += Depth)
            {
                int startValue = sorted[bStart];
                int endValue = sorted[bStart];
                int countValue = 1;

                for (int bIter = bStart + 1; bIter < bStart + Depth && bIter < sorted.Count; bIter++) {
                    countValue++;
                    endValue = sorted[bIter];
                }
                Buckets.Add(new HistogramBucket(startValue, endValue, countValue));
            }
        }

        public void GenerateHistogram(List<ValueCount> sortedGroups)
        {
            int? minValue = null;
            int? maxValue = null;
            long count = 0;

            foreach (var grp in sortedGroups) {
                if(minValue == null) // Begin new bucket
                    minValue = grp.Value;

                count += grp.Count;
                maxValue = grp.Value;

                if (count >= Depth)
                {
                    Buckets.Add(new HistogramBucket((int)minValue, grp.Value, count));
                    minValue = null;
                    maxValue = null;
                    count = 0;
                }
            }

            // Catch final value, if it wasn't enough to trigger a new bucket
            if(minValue != null)
            {
                Buckets.Add(new HistogramBucket((int)minValue, (int)maxValue!, count));
            }

        }

        public override string? ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"\t Data for attribute {TableName}.{AttributeName}:");
            foreach(var bucket in Buckets)
            {
                sb.AppendLine($"\t\t{bucket}");
            }
            return sb.ToString();
        }
    }
}