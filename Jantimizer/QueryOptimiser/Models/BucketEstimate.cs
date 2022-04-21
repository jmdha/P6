using Histograms.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryOptimiser.Models
{
    public class BucketEstimate
    {
        internal IHistogramBucket Bucket { get; set; }
        internal long Estimate { get; set; }

        public BucketEstimate(IHistogramBucket bucket, long estimatee)
        {
            Bucket = bucket;
            Estimate = estimatee;
        }
    }
}
