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

        internal IntermediateBucket(List<Tuple<TableReferenceNode, string, IHistogramBucket>> buckets)
        {
            AddBuckets(buckets);
        }

        internal IntermediateBucket(IntermediateBucket bucket1, IntermediateBucket bucket2)
        {
            AddBuckets(bucket1);
            AddBuckets(bucket2);
        }

        public IHistogramBucket GetBucket(TableReferenceNode node, string attribute)
        {
            return Buckets[node][attribute];
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
                    throw new ArgumentException("Can not add the same bucket twice(Should not occur)");
                Count *= bucket.Item3.Count;
            }
        }
    }
}
