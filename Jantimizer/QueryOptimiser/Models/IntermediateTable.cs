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

        public IntermediateTable(List<IntermediateBucket> buckets, List<Tuple<TableReferenceNode, List<string>>> references) {
            Buckets = buckets;
            foreach (var reference in references)
            {
                if (!References.ContainsKey(reference.Item1)) {
                    References.Add(reference.Item1, reference.Item2);
                    continue;
                }
                
                foreach (var attribute in reference.Item2)
                    if (!References[reference.Item1].Contains(attribute))
                        References[reference.Item1].Add(attribute);
            }
        }

        public long GetRowEstimate()
        {
            long estimate = 0;
            foreach (IntermediateBucket bucket in Buckets)
                estimate += bucket.GetCount();
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
            Tuple<TableReferenceNode, string>? overlap = null;
            foreach(var refe in it1.References)
            {
                it.References.Add(refe.Key, refe.Value);
            }
            foreach (var refe in it2.References)
            {
                if (it.References.ContainsKey(refe.Key))
                {
                    foreach (var attribute in refe.Value)
                        if (it.DoesContain(refe.Key, attribute))
                            overlap = new Tuple<TableReferenceNode, string>(refe.Key, attribute);
                } else
                    it.References.Add(refe.Key, refe.Value);
            }
            if (overlap != null)
                return JoinWithOverlap(it, it1, it2, overlap);
            else
                return JoinWithoutOverlap(it, it1, it2);
        }

        private static IntermediateTable JoinWithOverlap(IntermediateTable it, IntermediateTable it1, IntermediateTable it2, Tuple<TableReferenceNode, string> overlap)
        {
            foreach (var bucket1 in it1.Buckets)
            {
                if (!bucket1.DoesContain(overlap.Item1, overlap.Item2))
                    continue;
                foreach (var bucket2 in it2.Buckets)
                {
                    if (!bucket2.DoesContain(overlap.Item1, overlap.Item2))
                        continue;
                    if (IntermediateBucket.DoesOverlap(overlap.Item1, overlap.Item2, bucket1, bucket2))
                       it.Buckets.Add(IntermediateBucket.Merge(bucket1, bucket2));
                }
            }
            return it;
        }

        private static IntermediateTable JoinWithoutOverlap(IntermediateTable it, IntermediateTable it1, IntermediateTable it2)
        {
            foreach (var bucket1 in it1.Buckets)
                foreach (var bucket2 in it2.Buckets)
                    it.Buckets.Add(new IntermediateBucket(bucket1, bucket2));
            return it;
        }
    }
}
