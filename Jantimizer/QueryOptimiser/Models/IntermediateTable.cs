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
        // HashSet of table references to attributes.
        // Used as a reference as to what tables have been joined in order to create table
        internal HashSet<TableAttribute> References { get; }
        internal List<IntermediateBucket> Buckets { get; }

        public IntermediateTable()
        {
            Buckets = new List<IntermediateBucket>();
            References = new HashSet<TableAttribute>();
        }

        public IntermediateTable(List<IntermediateBucket> buckets, List<TableAttribute> references) {
            Buckets = buckets;
            References = new HashSet<TableAttribute>();
            foreach (var reference in references)
                References.Add(reference);
        }

        public long GetRowEstimate()
        {
            long estimate = 0;
            foreach (IntermediateBucket bucket in Buckets)
                estimate += bucket.GetEstimateOfAllBuckets();
            return estimate;
        }

        public List<IHistogramBucket> GetBuckets(TableAttribute tableAttribute)
        {
            List<IHistogramBucket> buckets = new List<IHistogramBucket>();
            foreach (var interBucket in Buckets)
                foreach (var bucketKeys in interBucket.Buckets.Keys)
                    if (bucketKeys.Equals(tableAttribute))
                        buckets.Add(interBucket.Buckets[bucketKeys].Bucket);
            buckets = buckets.DistinctBy(x => x.BucketId).ToList();
            return buckets;
        }
    }
}
