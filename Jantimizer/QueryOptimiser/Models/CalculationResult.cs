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
        public long Estimate { get; }
        internal BucketLimitation? BucketLimit { get; set; }

        internal CalculationResult(long estimate)
        {
            Estimate = estimate;
        }

        internal CalculationResult(long estimate, BucketLimitation bucketLimit)
        {
            Estimate = estimate;
            BucketLimit = bucketLimit;
        }
    }
}
