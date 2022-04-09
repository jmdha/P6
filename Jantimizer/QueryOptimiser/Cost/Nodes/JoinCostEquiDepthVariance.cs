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
            double certainty = (double)Math.Abs((double)rightVBucket.Variance / rightVBucket.Variance);
            if (certainty > 1)
                certainty = 1 / certainty;
            long estimate = (long)(rightVBucket.Count * certainty);
            if (estimate == 0)
                return 1;
            else
                return estimate;
        }
    }
}
