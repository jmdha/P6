using Histograms.Models;
using QueryParser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Tools.Models.Dictionaries;

[assembly:InternalsVisibleTo("QueryOptimiserTest")]

namespace QueryOptimiser.Models
{
    public class IntermediateBucket
    {
        internal DualDictionary<TableAttribute, BucketEstimate> Buckets { get; } = new DualDictionary<TableAttribute, BucketEstimate>();

        public IntermediateBucket() { }

        public IntermediateBucket(IntermediateBucket bucket1, IntermediateBucket bucket2)
        {
            AddBuckets(bucket1);
            AddBuckets(bucket2);
        }

        internal long GetEstimateOfAllBuckets()
        {
            long count = 1;
            foreach (var tableAttribute in Buckets.Keys)
                count *= Buckets[tableAttribute].Estimate;
            return count;
        }

        internal void AddBuckets(IntermediateBucket bucket)
        {
            foreach (var tableAttribute in bucket.Buckets.Keys)
                AddBucket(tableAttribute, bucket.Buckets[tableAttribute]);
        }

        internal void AddBucket(TableAttribute tableAttribute, BucketEstimate bucket, bool throwOnDuplicate = true)
        {
            if (!Buckets.DoesContain(tableAttribute))
                Buckets.Add(tableAttribute, bucket);
            else
                if (throwOnDuplicate)
                    throw new ArgumentException("Can not add the same bucket twice");
        }
    }
}
