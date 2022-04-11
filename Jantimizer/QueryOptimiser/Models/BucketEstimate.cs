using Histograms.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryOptimiser.Models
{
    internal class BucketEstimate
    {
        internal IHistogramBucket Bucket { get; set; }
        internal long Estimatee { get; set; }

        public BucketEstimate(IHistogramBucket bucket, long estimatee)
        {
            Bucket = bucket;
            Estimatee = estimatee;
        }
    }
}
