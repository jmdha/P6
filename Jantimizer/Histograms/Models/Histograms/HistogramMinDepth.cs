using Histograms.Caches;
using System.Data;
using System.Text;

namespace Histograms.Models
{
    /// <summary>
    /// Contains n buckets, which each represent the same m number of elements.
    /// </summary>
    public class HistogramMinDepth : BaseHistogram, IDepthHistogram
    {
        public override List<TypeCode> AcceptedTypes { get; } = new List<TypeCode>() {
            TypeCode.String,
            TypeCode.DateTime,
            TypeCode.Double,
            TypeCode.Decimal,
            TypeCode.Int16,
            TypeCode.Int32,
            TypeCode.Int64,
            TypeCode.UInt16,
            TypeCode.UInt32,
            TypeCode.UInt64,
        };
        public int Depth { get; }

        public HistogramMinDepth(string tableName, string attributeName, int depth) : base(tableName, attributeName)
        {
            HistogramId = Guid.NewGuid();
            Depth = depth;
        }

        public HistogramMinDepth(Guid histogramId, string tableName, string attributeName, int depth) : base(histogramId, tableName, attributeName)
        {
            Depth = depth;
        }

        protected override void GenerateHistogramFromSorted(List<IComparable> sorted)
        {
            GenerateHistogramFromSortedGroups(
                sorted.GroupBy(x => x).Select(grp => new ValueCount(grp.Key, grp.Count())).ToList()
            );
        }

        public override void GenerateHistogramFromSortedGroups(IEnumerable<ValueCount> sortedGroups)
        {
            IComparable? minValue = null;
            IComparable? maxValue = null;
            long count = 0;

            foreach (var grp in sortedGroups)
            {
                if (minValue == null) // Begin new bucket
                    minValue = grp.Value;

                count += grp.Count;
                maxValue = grp.Value;

                if (count >= Depth)
                {
                    Buckets.Add(new HistogramBucket(minValue, grp.Value, count));
                    minValue = null;
                    maxValue = null;
                    count = 0;
                }
            }

            // Catch final value, if it wasn't enough to trigger a new bucket
            if (minValue != null || maxValue != null)
            {
                if (minValue == null || maxValue == null)
                    throw new NullReferenceException($"Unexpected null, should be impossible for minValue or maxValue to be null here");

                Buckets.Add(new HistogramBucket(minValue, maxValue, count));
            }
        }

        public override object Clone()
        {
            var retObj = new HistogramMinDepth(HistogramId, TableName, AttributeName, Depth);
            foreach (var bucket in Buckets)
                if (bucket.Clone() is IHistogramBucket acc)
                    retObj.Buckets.Add(acc);
            return retObj;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() + HashCode.Combine(Depth);
        }
    }
}