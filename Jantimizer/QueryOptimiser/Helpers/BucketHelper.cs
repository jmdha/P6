using QueryOptimiser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryOptimiser.Helpers
{
    public class BucketHelper
    {
        internal static IntermediateBucket Merge(IntermediateBucket bucket1, IntermediateBucket bucket2)
        {
            IntermediateBucket bucket = new IntermediateBucket();
            foreach (var tableAttribute in bucket1.Buckets.Keys)
                bucket.AddBucket(tableAttribute, bucket1.Buckets[tableAttribute], false);
            foreach (var tableAttribute in bucket2.Buckets.Keys)
                bucket.AddBucket(tableAttribute, bucket2.Buckets[tableAttribute], false);
            return bucket;
        }

        internal static List<IntermediateBucket> MergeOnOverlap(List<IntermediateBucket> buckets1, List<IntermediateBucket> buckets2)
        {
            List<IntermediateBucket> buckets = new List<IntermediateBucket>();
            foreach (var bucket1 in buckets1)
            {
                foreach (var tableAttribute in bucket1.Buckets.Keys)
                {
                    foreach (var bucket2 in buckets2)
                    {
                        if (DoesOverlap(tableAttribute, bucket1, bucket2))
                        {
                            var newBucket = new IntermediateBucket();
                            newBucket.AddBucket(tableAttribute, bucket1.Buckets[tableAttribute]);
                            buckets.Add(newBucket);
                            continue;
                        }
                    }
                }
            }
            return buckets;
        }

        internal static bool DoesOverlap(TableAttribute tableAttribute, IntermediateBucket bucket1, IntermediateBucket bucket2)
        {
            if (bucket1.Buckets.DoesContain(tableAttribute) && bucket2.Buckets.DoesContain(tableAttribute))
            {
                if (bucket1.Buckets[tableAttribute].Bucket == bucket2.Buckets[tableAttribute].Bucket)
                    return true;
                else
                    return false;
            }
            else
                return false;
        }
    }
}
