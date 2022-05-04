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

            Queue<ValueCount> groupQueue = new Queue<ValueCount>(sortedGroups);

            while (groupQueue.Count > 0)
            {
                ValueCount currentGrp = groupQueue.Dequeue();

                IComparable minValue = currentGrp.Value;
                long count = currentGrp.Count;

                while (count < depth && groupQueue.Count() > 0)
                {
                    count += groupQueue.Dequeue().Count;
                }

                Buckets.Add(new HistogramBucket(minValue, currentGrp.Value, count));
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