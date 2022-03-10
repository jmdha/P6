using System.Data;

namespace Histograms
{
    /// <summary>
    /// Contains n buckets, which each represent the same m number of elements.
    /// </summary>
    public class HistogramEquiDepth : IHistogram
    {
        public List<IHistogramBucket> Buckets { get; }
        public int Depth { get; }

        public HistogramEquiDepth(int depth)
        {
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
            for (int i = 0; i < sorted.Count; i += Depth)
            {
                IHistogramBucket newBucket = new HistogramBucket(sorted[i], 1);
                for (int j = i + 1; j <= Depth && j < sorted.Count; j++)
                    newBucket.Count++;
                Buckets.Add(newBucket);
            }
        }
    }
}