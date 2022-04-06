using Histograms.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryOptimiser.Cost.Models
{
    internal class CostCalculationResult
    {
        public long Cost { get; set; }
        public IHistogramBucket LeftBuckets { get; set; }
        public IHistogramBucket RightBuckets { get; set; }
    }
}
