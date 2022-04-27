using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.Models.JsonModels;

namespace QueryOptimiser.Models
{
    public class BucketMatches
    {
        public List<IntermediateBucket> Buckets { get; set; }
        public List<TableAttribute> References { get; set; }

        public BucketMatches()
        {
            Buckets = new List<IntermediateBucket>();
            References = new List<TableAttribute>();
        }

        public BucketMatches(List<IntermediateBucket> buckets, List<TableAttribute> references)
        {
            Buckets = buckets;
            References = references;
        }
    }
}
