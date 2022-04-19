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

        public IntermediateTable(List<IntermediateBucket> buckets, List<Tuple<TableReferenceNode, string>> references) {
            Buckets = buckets;
            foreach (var reference in references)
            {
                if (!References.ContainsKey(reference.Item1)) {
                    References.Add(reference.Item1, new List<string> { reference.Item2 });
                    continue;
                } else if (!References[reference.Item1].Contains(reference.Item2))
                    References[reference.Item1].Add(reference.Item2);
            }
        }

        public long GetRowEstimate()
        {
            long estimate = 0;
            foreach (IntermediateBucket bucket in Buckets)
                estimate += bucket.GetEstimateOfAllBuckets();
            return estimate;
        }

        public bool DoesContain(TableReferenceNode refNode, string attribute)
        {
            return (References.ContainsKey(refNode) && References[refNode].Contains(attribute));
        }

        public List<IHistogramBucket> GetBuckets(string table, string attribute)
        {
            List<IHistogramBucket> buckets = new List<IHistogramBucket>();
            foreach (IntermediateBucket bucket in Buckets)
                buckets.Add(bucket.GetBucket(table, attribute));
            return buckets;
        }

        public static IntermediateTable Join(IntermediateTable it1, IntermediateTable it2) 
        {
            IntermediateTable it = new IntermediateTable();
            string? table = null;
            string? attribute = null;
            foreach(var refe in it1.References)
            {
                it.References.Add(refe.Key, refe.Value);
            }
            foreach (var refe in it2.References)
            {
                if (it.References.ContainsKey(refe.Key))
                {
                    table = refe.Key.Alias;
                    foreach (var att in refe.Value)
                        if (it.DoesContain(refe.Key, att))
                            attribute = att;
                } else
                    it.References.Add(refe.Key, refe.Value);
            }
            if (table != null && attribute != null)
                return JoinWithOverlap(it, it1, it2, table, attribute);
            else
                return JoinWithoutOverlap(it, it1, it2);
        }

        private static IntermediateTable JoinWithOverlap(IntermediateTable it, IntermediateTable it1, IntermediateTable it2, string table, string attribute)
        {
            foreach (var bucket1 in it1.Buckets)
            {
                if (!bucket1.Buckets.DoesContain(table, attribute))
                    continue;
                foreach (var bucket2 in it2.Buckets)
                {
                    if (!bucket2.Buckets.DoesContain(table, attribute))
                        continue;
                    if (IntermediateBucket.DoesOverlap(table, attribute, bucket1, bucket2))
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
