using System.Data;
using System.Text;

namespace Histograms.Models
{
    /// <summary>
    /// Contains n buckets, which each represent the same m number of elements.
    /// </summary>
    public class HistogramEquiDepth : BaseHistogram, IDepthHistogram
    {
        public int Depth { get; }

        public HistogramEquiDepth(string tableName, string attributeName, int depth) : base(tableName, attributeName)
        {
            Depth = depth;
        }


        protected override void GenerateHistogramFromSorted(List<IComparable> sorted)
        {
            for (int bStart = 0; bStart < sorted.Count; bStart += Depth)
            {
                IComparable startValue = sorted[bStart];
                IComparable endValue = sorted[bStart];
                int countValue = 1;

                for (int bIter = bStart + 1; bIter < bStart + Depth && bIter < sorted.Count; bIter++)
                {
                    countValue++;
                    endValue = sorted[bIter];
                }
                Buckets.Add(new HistogramBucket(startValue, endValue, countValue));
            }
        }

        public override void GenerateHistogramFromSortedGroups(IEnumerable<ValueCount> sortedGroups)
        {
            GenerateHistogramFromSorted(SplitValues(sortedGroups).ToList());
        }

        private IEnumerable<IComparable> SplitValues(IEnumerable<ValueCount> sortedGroups)
        {
            foreach (var grp in sortedGroups)
                for (int i = 0; i < grp.Count; i++)
                    yield return grp.Value;
        }
    }
}