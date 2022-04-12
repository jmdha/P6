using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QueryParser.Models;
using Histograms;
using Histograms.Models;
using DatabaseConnector;

namespace QueryOptimiser.Cost.Nodes.EquiDepthVariance
{
    internal class JoinCostEquiDepthVariance : BaseJoinCost
    {
        public override long GetBucketEstimate(ComparisonType.Type predicate, IHistogramBucket bucket, IHistogramBucket comparisonBucket)
        {
            HistogramBucketVariance vBucket = (HistogramBucketVariance)bucket;
            HistogramBucketVariance vComparisonBucket = (HistogramBucketVariance)comparisonBucket;
            double certainty = ((double)vBucket.StandardDeviation / vBucket.Range) / ((double)vComparisonBucket.StandardDeviation / vComparisonBucket.Range);
            if (certainty > 1)
                certainty = 1 / certainty;
            long estimate = (long)(certainty * vBucket.Count);
            if (estimate == 0)
                return 1;
            else
                return estimate;
        }
    }
}
