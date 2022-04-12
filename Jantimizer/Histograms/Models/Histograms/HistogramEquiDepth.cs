using Histograms.Caches;
using System.Data;
using System.Text;

namespace Histograms.Models
{
    /// <summary>
    /// Contains n buckets, which each represent the same m number of elements.
    /// </summary>
    public class HistogramEquiDepth : BaseHistogram, IDepthHistogram
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

        public HistogramEquiDepth(string tableName, string attributeName, int depth) : base(tableName, attributeName)
        {
            Depth = depth;
        }

        internal HistogramEquiDepth(CachedHistogram histo) : base(histo.TableName, histo.AttributeName)
        {
            Depth = histo.Depth;
            foreach (var bucket in histo.Buckets)
            {
                Type? type = Type.GetType(bucket.ValueType);
                if (type == null)
                    throw new NullReferenceException("Unexpected null as cache type");


                if (type.GetInterface(nameof(IComparable)) != null)
                {
                    var valueStart = Convert.ChangeType(bucket.ValueStart, type) as IComparable;
                    var valueEnd = Convert.ChangeType(bucket.ValueEnd, type) as IComparable;

                    if (valueStart == null || valueEnd == null)
                        throw new ArgumentNullException("Read bucket value was invalid!");

                    Buckets.Add(new HistogramBucket(valueStart, valueEnd, bucket.Count));
                }
            }
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

        public override object Clone()
        {
            var retObj = new HistogramEquiDepth(TableName, AttributeName, Depth);
            foreach (var bucket in Buckets)
                retObj.Buckets.Add(new HistogramBucket(bucket.ValueStart, bucket.ValueEnd, bucket.Count));
            return retObj;
        }
    }
}