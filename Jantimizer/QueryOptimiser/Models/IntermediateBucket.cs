using Histograms.Models;
using QueryParser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryOptimiser.Models
{
    public class IntermediateBucket
    {
        internal Dictionary<TableReferenceNode, Dictionary<string, BucketEstimate>> Buckets { get; } = new Dictionary<TableReferenceNode, Dictionary<string, BucketEstimate>>();

        internal IntermediateBucket() { }
        internal IntermediateBucket(List<Tuple<TableReferenceNode, string, BucketEstimate>> buckets)
        {
            AddBuckets(buckets);
        }

        internal IntermediateBucket(IntermediateBucket bucket1, IntermediateBucket bucket2)
        {
            AddBuckets(bucket1);
            AddBuckets(bucket2);
        }

        internal static IntermediateBucket Merge(IntermediateBucket bucket1, IntermediateBucket bucket2)
        {
            IntermediateBucket bucket = new IntermediateBucket();
            foreach (var tableRef in bucket1.Buckets)
                foreach (var attribute in tableRef.Value)
                    bucket.AddBucketIgnoreDuplicates(Tuple.Create(tableRef.Key, attribute.Key, attribute.Value));
            foreach (var tableRef in bucket2.Buckets)
                foreach (var attribute in tableRef.Value)
                    bucket.AddBucketIgnoreDuplicates(Tuple.Create(tableRef.Key, attribute.Key, attribute.Value));
            return bucket;
        }

        internal static List<IntermediateBucket> MergeOnOverlap(List<IntermediateBucket> buckets1, List<IntermediateBucket> buckets2)
        {
            List<IntermediateBucket> buckets = new List<IntermediateBucket>();
            foreach (var bucket in buckets1) {
                foreach (var tableRef in bucket.Buckets) {
                    foreach (var attribute in tableRef.Value) {
                        foreach (var bucket2 in buckets2) {
                            if (DoesOverlap(tableRef.Key, attribute.Key, bucket, bucket2))
                            {
                                buckets.Add(MergeOnOverlap(bucket, bucket2));
                                continue;
                            }
                        }
                    }
                }
            }
            return buckets;
        }

        internal static IntermediateBucket MergeOnOverlap(IntermediateBucket bucket1, IntermediateBucket bucket2)
        {
            IntermediateBucket bucket = new IntermediateBucket();
            foreach (var tableRef in bucket1.Buckets)
                foreach (var attribute in tableRef.Value)
                    if (DoesOverlap(tableRef.Key, attribute.Key, bucket1, bucket2))
                        bucket.AddBucket(Tuple.Create(tableRef.Key, attribute.Key, attribute.Value));
            return bucket;
        }

        internal static bool DoesOverlap(TableReferenceNode node, string attribute, IntermediateBucket bucket1, IntermediateBucket bucket2)
        {
            if (bucket1.DoesContain(node, attribute) && bucket2.DoesContain(node, attribute)) {
                if (bucket1.GetBucket(node, attribute) == bucket2.GetBucket(node, attribute))
                    return true;
                else
                    return false;
            } else
                return false;
        }

        internal IHistogramBucket GetBucket(TableReferenceNode node, string attribute)
        {
            return Buckets[node][attribute].Bucket;
        }

        internal bool DoesContain(TableReferenceNode node, string attribute)
        {
            return (Buckets.ContainsKey(node) && Buckets[node].ContainsKey(attribute));
        }

        internal long GetCount()
        {
            long count = 1;
            foreach (var bucket in Buckets)
                foreach (var attribute in bucket.Value)
                    count *= attribute.Value.Estimate;
            return count;
        }

        private void AddBuckets(IntermediateBucket bucket)
        {
            foreach (var refe in bucket.Buckets)
                foreach (var refedBucket in refe.Value)
                    AddBucket(new Tuple<TableReferenceNode, string, BucketEstimate>(refe.Key, refedBucket.Key, refedBucket.Value));
        }

        private void AddBuckets(List<Tuple<TableReferenceNode, string, BucketEstimate>> buckets)
        {
            foreach (var bucket in buckets)
            {
                AddBucket(bucket);
            }
        }

        private void AddBucket(Tuple<TableReferenceNode, string, BucketEstimate> bucket)
        {
            if (!Buckets.ContainsKey(bucket.Item1))
                Buckets.Add(bucket.Item1, new Dictionary<string, BucketEstimate>());
            if (!Buckets[bucket.Item1].ContainsKey(bucket.Item2))
                Buckets[bucket.Item1].Add(bucket.Item2, bucket.Item3);
            else
                throw new ArgumentException("Can not add the same bucket twice");
        }

        private void AddBucketIgnoreDuplicates(Tuple<TableReferenceNode, string, BucketEstimate> bucket)
        {
            if (!Buckets.ContainsKey(bucket.Item1))
                Buckets.Add(bucket.Item1, new Dictionary<string, BucketEstimate>());
            if (!Buckets[bucket.Item1].ContainsKey(bucket.Item2))
                Buckets[bucket.Item1].Add(bucket.Item2, bucket.Item3);
        }
    }
}
