using Histograms;
using Histograms.Models;
using QueryOptimiser.Helpers;
using QueryParser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryOptimiser.Models
{
    public class IntermediateTable
    {
        // Dictionary of table references to attributes.
        // Used as a reference as to what tables have been joined in order to create table
        // Table, List of attributes
        internal HashSet<TableAttribute> _References { get; } = new HashSet<TableAttribute>();
        internal List<IntermediateBucket> Buckets { get; }

        public IntermediateTable()
        {
            Buckets = new List<IntermediateBucket>();
        }

        public IntermediateTable(List<IntermediateBucket> buckets, List<TableAttribute> references) {
            Buckets = buckets;
            foreach (var reference in references)
            {
                if (!_References.Contains(reference)) {
                    _References.Add(reference);
                    continue;
                }
            }
        }

        public long GetRowEstimate()
        {
            long estimate = 0;
            foreach (IntermediateBucket bucket in Buckets)
                estimate += bucket.GetEstimateOfAllBuckets();
            return estimate;
        }

        public bool DoesContain(TableAttribute tableAttribute)
        {
            return _References.Contains(tableAttribute);
        }

        public List<IHistogramBucket> GetBuckets(TableAttribute tableAttribute)
        {
            List<IHistogramBucket> buckets = new List<IHistogramBucket>();
            foreach (IntermediateBucket bucket in Buckets)
                buckets.Add(bucket.Buckets[tableAttribute].Bucket);
            return buckets;
        }
    }
}
