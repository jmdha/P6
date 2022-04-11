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
        public override long GetCombinedEstimate(ComparisonType.Type predicate, IHistogramBucket leftBucket, IHistogramBucket rightBucket)
        {
            HistogramBucketVariance leftVBucket = (HistogramBucketVariance)leftBucket;
            HistogramBucketVariance rightVBucket = (HistogramBucketVariance)rightBucket;
            double certainty = ((double)leftVBucket.Variance / leftVBucket.Range) / ((double)rightVBucket.Variance / rightVBucket.Range);
            if (certainty > 1)
                certainty = 1 / certainty;
            long estimate = (long)(certainty * (leftVBucket.Count * rightVBucket.Count));
            if (estimate == 0)
                return 1;
            else
                return estimate;
        }
    }
}
