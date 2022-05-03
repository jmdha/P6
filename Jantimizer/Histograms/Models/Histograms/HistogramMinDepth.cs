using Histograms.Caches;
using Histograms.DepthCalculators;
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
        public IDepthCalculator DepthCalculator { get; }

        public HistogramMinDepth(string tableName, string attributeName, IDepthCalculator depthCalculator) : base(tableName, attributeName)
        {
            HistogramId = Guid.NewGuid();
            DepthCalculator = depthCalculator;
        }

        public HistogramMinDepth(Guid histogramId, string tableName, string attributeName, IDepthCalculator getDepth) : base(histogramId, tableName, attributeName)
        {
            DepthCalculator = getDepth;
        }

        public override void GenerateHistogramFromSortedGroups(IEnumerable<ValueCount> sortedGroups)
        {
            var depth = DepthCalculator.GetDepth(sortedGroups.Count(), sortedGroups.Sum(x => x.Count));

            IComparable? minValue = null;
            IComparable? maxValue = null;
            long count = 0;

            foreach (var grp in sortedGroups)
            {
                if (minValue == null) // Begin new bucket
                    minValue = grp.Value;

                count += grp.Count;
                maxValue = grp.Value;

                if (count >= depth)
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
            var retObj = new HistogramMinDepth(HistogramId, TableName, AttributeName, DepthCalculator);
            foreach (var bucket in Buckets)
                if (bucket.Clone() is IHistogramBucket acc)
                    retObj.Buckets.Add(acc);
            return retObj;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() + HashCode.Combine(DepthCalculator);
        }
    }
}