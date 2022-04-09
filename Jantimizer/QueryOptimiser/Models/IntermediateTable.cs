using Histograms;
using Histograms.Models;
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
        internal Dictionary<TableReferenceNode, List<string>> References { get; } = new Dictionary<TableReferenceNode, List<string>>();
        internal List<IntermediateBucket> Buckets { get; }

        public IntermediateTable()
        {
            Buckets = new List<IntermediateBucket>();
        }

        public IntermediateTable(List<IntermediateBucket> buckets) {
            Buckets = buckets;
        }

        public long GetRowEstimate()
        {
            long estimate = 0;
            foreach (IntermediateBucket bucket in Buckets)
                estimate += bucket.Count;
            return estimate;
        }

        public bool DoesContain(TableReferenceNode refNode, string attribute)
        {
            return (References.ContainsKey(refNode) && References[refNode].Contains(attribute));
        }

        public List<IHistogramBucket> GetBuckets(TableReferenceNode node, string attribute)
        {
            List<IHistogramBucket> buckets = new List<IHistogramBucket>();
            foreach (IntermediateBucket bucket in Buckets)
                buckets.Add(bucket.GetBucket(node, attribute));
            return buckets;
        }

        public static IntermediateTable Join(IntermediateTable it1, IntermediateTable it2) 
        {
            IntermediateTable it = new IntermediateTable();
            foreach(var refe in it1.References)
            {
                it.References.Add(refe.Key, refe.Value);
            }
            bool overlap = false;
            foreach (var refe in it2.References)
            {
                if (it.References.ContainsKey(refe.Key))
                    overlap = true;
                else
                    it.References.Add(refe.Key, refe.Value);
            }
            if (overlap)
                return JoinWithOverlap(it, it1, it2);
            else
                return JoinWithoutOverlap(it, it1, it2);
        }

        private static IntermediateTable JoinWithOverlap(IntermediateTable it, IntermediateTable it1, IntermediateTable it2)
        {
            // To be implemented
            return JoinWithoutOverlap(it, it1, it2);
        }

        private static IntermediateTable JoinWithoutOverlap(IntermediateTable it, IntermediateTable it1, IntermediateTable it2)
        {
            List<IntermediateBucket> buckets = new List<IntermediateBucket>();
            foreach (var bucket1 in it1.Buckets)
            {
                foreach (var bucket2 in it2.Buckets)
                {
                    buckets.Add(new IntermediateBucket(bucket1, bucket2));
                }
            }
            it.Buckets.AddRange(buckets);
            return it;
        }
    }
}
