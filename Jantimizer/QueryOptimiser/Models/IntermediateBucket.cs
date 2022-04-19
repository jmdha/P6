using Histograms.Models;
using QueryParser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Tools.Models;

[assembly:InternalsVisibleTo("QueryOptimiserTest")]

namespace QueryOptimiser.Models
{
    public class IntermediateBucket
    {
        // Table, Attribute, Bucket estimate
        internal MultiDictionary<string, string, BucketEstimate> Buckets { get; } = new MultiDictionary<string, string, BucketEstimate>();

        public IntermediateBucket() { }

        public IntermediateBucket(IntermediateBucket bucket1, IntermediateBucket bucket2)
        {
            AddBuckets(bucket1);
            AddBuckets(bucket2);
        }

        internal static IntermediateBucket Merge(IntermediateBucket bucket1, IntermediateBucket bucket2)
        {
            IntermediateBucket bucket = new IntermediateBucket();
            foreach (var tableRef in bucket1.Buckets.Keys)
                foreach (var attributeRef in bucket1.Buckets[tableRef])
                    bucket.AddBucketIgnoreDuplicates(tableRef, attributeRef, bucket1.Buckets[tableRef, attributeRef]);
            foreach (var tableRef in bucket2.Buckets.Keys)
                foreach (var attributeRef in bucket2.Buckets[tableRef])
                    bucket.AddBucketIgnoreDuplicates(tableRef, attributeRef, bucket2.Buckets[tableRef, attributeRef]);
            return bucket;
        }

        internal static List<IntermediateBucket> MergeOnOverlap(List<IntermediateBucket> buckets1, List<IntermediateBucket> buckets2)
        {
            List<IntermediateBucket> buckets = new List<IntermediateBucket>();
            foreach (var bucket1 in buckets1) {
                foreach (var table in bucket1.Buckets.Keys) {
                    foreach (var attribute in bucket1.Buckets[table]) {
                        foreach (var bucket2 in buckets2) {
                            if (DoesOverlap(table, attribute, bucket1, bucket2))
                            {
                                var newBucket = new IntermediateBucket();
                                newBucket.AddBucket(table, attribute, bucket1.Buckets[table, attribute]);
                                buckets.Add(newBucket);
                                continue;
                            }
                        }
                    }
                }
            }
            return buckets;
        }

        internal static bool DoesOverlap(string table, string attribute, IntermediateBucket bucket1, IntermediateBucket bucket2)
        {
            if (bucket1.Buckets.DoesContain(table, attribute) && bucket2.Buckets.DoesContain(table, attribute)) {
                if (bucket1.GetBucket(table, attribute) == bucket2.GetBucket(table, attribute))
                    return true;
                else
                    return false;
            } else
                return false;
        }

        internal IHistogramBucket GetBucket(string tableRef, string attributeRef)
        {
            return Buckets[tableRef, attributeRef].Bucket;
        }

        internal long GetEstimateOfAllBuckets()
        {
            long count = 1;
            foreach (var table in Buckets.Keys)
                foreach (var attribute in Buckets[table])
                    count *= Buckets[table, attribute].Estimate;
            return count;
        }

        internal void AddBuckets(IntermediateBucket bucket)
        {
            foreach (var table in bucket.Buckets.Keys)
                foreach (var attribute in bucket.Buckets[table])
                    AddBucket(table, attribute, bucket.Buckets[table, attribute]);
        }

        internal void AddBucket(string table, string attribute, BucketEstimate bucket)
        {
            if (!Buckets.DoesContain(table, attribute))
                Buckets.Add(table, attribute, bucket);
            else
                throw new ArgumentException("Can not add the same bucket twice");
        }

        internal void AddBucketIgnoreDuplicates(string table, string attribute, BucketEstimate bucket)
        {
            if (!Buckets.DoesContain(table, attribute))
                Buckets.Add(table, attribute, bucket);
        }
    }
}
