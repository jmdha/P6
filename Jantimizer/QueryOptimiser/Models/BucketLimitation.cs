using Histograms.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryOptimiser.Models
{
    internal class BucketLimitation
    {
        internal BucketDictionary PrimaryBuckets { get; }
        internal BucketDictionary AlternatveBuckets { get; }

        internal BucketLimitation(BucketDictionary primaryBuckets)
        {
            PrimaryBuckets = primaryBuckets;
            AlternatveBuckets = new BucketDictionary();
        }

        internal BucketLimitation(BucketDictionary primaryBuckets, BucketDictionary alternatveBuckets)
        {
            PrimaryBuckets = primaryBuckets;
            AlternatveBuckets = alternatveBuckets;
        }
    }
}
