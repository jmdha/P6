using QueryOptimiser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryOptimiser.Helpers
{
    internal static class TableHelper
    {
        internal static IntermediateTable Join(IntermediateTable it1, IntermediateTable it2)
        {
            IntermediateTable it = new IntermediateTable();
            TableAttribute? tableAttribute = null;
            foreach (var refe in it1.References)
                it.References.Add(refe);
            
            foreach (var refe in it2.References)
            {
                if (it.References.Contains(refe))
                    tableAttribute = refe;
                else
                    it.References.Add(refe);
            }
            if (tableAttribute != null)
                return JoinWithOverlap(it, it1, it2, tableAttribute);
            else
                return JoinWithoutOverlap(it, it1, it2);
        }

        internal static IntermediateTable JoinWithOverlap(IntermediateTable it, IntermediateTable it1, IntermediateTable it2, TableAttribute tableAttribute)
        {
            foreach (var bucket1 in it1.Buckets)
            {
                if (!bucket1.Buckets.DoesContain(tableAttribute))
                    continue;
                foreach (var bucket2 in it2.Buckets)
                {
                    if (!bucket2.Buckets.DoesContain(tableAttribute))
                        continue;
                    if (BucketHelper.DoesOverlap(tableAttribute, bucket1, bucket2))
                        it.Buckets.Add(BucketHelper.Merge(bucket1, bucket2));
                }
            }
            return it;
        }

        internal static IntermediateTable JoinWithoutOverlap(IntermediateTable it, IntermediateTable it1, IntermediateTable it2)
        {
            foreach (var bucket1 in it1.Buckets)
                foreach (var bucket2 in it2.Buckets)
                    it.Buckets.Add(new IntermediateBucket(bucket1, bucket2));
            return it;
        }
    }
}
