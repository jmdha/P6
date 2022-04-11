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
        internal long Count { get; private set; }
        internal Dictionary<TableReferenceNode, Dictionary<string, IHistogramBucket>> Buckets { get; } = new Dictionary<TableReferenceNode, Dictionary<string, IHistogramBucket>>();

        internal IntermediateBucket() { }
        internal IntermediateBucket(List<Tuple<TableReferenceNode, string, IHistogramBucket>> buckets, long count)
        {
            AddBuckets(buckets);
            Count = count;
        }

        internal IntermediateBucket(IntermediateBucket bucket1, IntermediateBucket bucket2)
        {
            AddBuckets(bucket1);
            AddBuckets(bucket2);
            Count = bucket1.Count * bucket2.Count;
        }

        internal static IntermediateBucket Merge(IntermediateBucket bucket1, IntermediateBucket bucket2)
        {
            IntermediateBucket bucket = new IntermediateBucket();
            foreach (var tableRef in bucket1.Buckets)
                foreach (var attribute in tableRef.Value)
                    bucket.AddBucketsIgnoreDuplicates(Tuple.Create(tableRef.Key, attribute.Key, attribute.Value));
            foreach (var tableRef in bucket2.Buckets)
                foreach (var attribute in tableRef.Value)
                    bucket.AddBucketsIgnoreDuplicates(Tuple.Create(tableRef.Key, attribute.Key, attribute.Value));
            bucket.Count = bucket1.Count * bucket2.Count;
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
            return Buckets[node][attribute];
        }

        internal bool DoesContain(TableReferenceNode node, string attribute)
        {
            return (Buckets.ContainsKey(node) && Buckets[node].ContainsKey(attribute));
        }

        private void AddBuckets(IntermediateBucket bucket)
        {
            List<Tuple<TableReferenceNode, string, IHistogramBucket>> tuples = new List<Tuple<TableReferenceNode, string, IHistogramBucket>>();
            foreach (var refe in bucket.Buckets)
                foreach (var refedBucket in refe.Value)
                    tuples.Add(new Tuple<TableReferenceNode, string, IHistogramBucket>(refe.Key, refedBucket.Key, refedBucket.Value));
            AddBuckets(tuples);
        }

        private void AddBuckets(List<Tuple<TableReferenceNode, string, IHistogramBucket>> buckets)
        {
            Count = 1;
            foreach (var bucket in buckets)
            {
                if (!Buckets.ContainsKey(bucket.Item1))
                    Buckets.Add(bucket.Item1, new Dictionary<string, IHistogramBucket>());
                if (!Buckets[bucket.Item1].ContainsKey(bucket.Item2))
                    Buckets[bucket.Item1].Add(bucket.Item2, bucket.Item3);
                else
                    throw new ArgumentException("Can not add the same bucket twice");
                Count *= bucket.Item3.Count;
            }
        }

        private void AddBucketsIgnoreDuplicates(Tuple<TableReferenceNode, string, IHistogramBucket> bucket)
        {
            if (!Buckets.ContainsKey(bucket.Item1))
                Buckets.Add(bucket.Item1, new Dictionary<string, IHistogramBucket>());
            if (!Buckets[bucket.Item1].ContainsKey(bucket.Item2))
                Buckets[bucket.Item1].Add(bucket.Item2, bucket.Item3);
        }
    }
}
