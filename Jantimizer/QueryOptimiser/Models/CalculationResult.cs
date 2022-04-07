using Histograms.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryOptimiser.Models
{
    public class CalculationResult
    {
        internal long Estimate { get; }
        internal BucketLimitation? BucketLimit { get; }

        internal CalculationResult(long estimate)
        {
            Estimate = estimate;
        }

        internal CalculationResult(long estimate, BucketLimitation bucketLimit)
        {
            Estimate = estimate;
            BucketLimit = bucketLimit;
        }

        internal CalculationResult(long estimate, BucketLimitation? bucketLimit1, BucketLimitation? bucketLimit2)
        {
            Estimate = estimate;
            if (bucketLimit1 != null && bucketLimit2 != null)
                BucketLimit = new BucketLimitation(bucketLimit1.PrimaryBuckets, bucketLimit2.PrimaryBuckets);
            else if (bucketLimit1 != null)
                BucketLimit = new BucketLimitation(bucketLimit1.PrimaryBuckets);
            else if (bucketLimit2 != null)
                BucketLimit = new BucketLimitation(bucketLimit2.PrimaryBuckets);
            else
                BucketLimit = null;
        }
    }
}
