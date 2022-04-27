using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Histograms;
using Histograms.Models;
using DatabaseConnector;
using System.Runtime.CompilerServices;
using QueryOptimiser.Cost.Calculations;
using Tools.Models.JsonModels;

namespace QueryOptimiser.Cost.Nodes
{
    public class FilterEstimateEquiDepthVariance : IFilterEstimate
    {
        public long GetBucketEstimate(ComparisonType.Type comparisonType, IComparable constant, IHistogramBucket bucket)
        {
            if (bucket is HistogramBucketVariance vBucket)
            {
                double bucketCertainty = DeviationEstimate.GetCertainty(vBucket.StandardDeviation, vBucket.Range);

                double certainty = DeviationEstimate.GetComparativeCertainty(bucketCertainty, 1);

                long estimate = (long)(certainty * vBucket.Count);
                if (estimate == 0)
                    return 1;
                else
                    return estimate;
            } else
            {
                return new FilterEstimateEquiDepth().GetBucketEstimate(comparisonType, constant, bucket);
            }
        }
    }
}
