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
    public class JoinEstimateEquiDepthVariance : IJoinEstimate
    {
        public long GetBucketEstimate(ComparisonType.Type predicate, IHistogramBucket bucket, IHistogramBucket comparisonBucket)
        {
            if (bucket is HistogramBucketVariance vBucket && comparisonBucket is HistogramBucketVariance vComparisonBucket)
            {
                double bucketCertainty = DeviationEstimate.GetCertainty(vBucket.StandardDeviation, vBucket.Range);
                double comparisonBucketCertainty = DeviationEstimate.GetCertainty(vComparisonBucket.StandardDeviation, vComparisonBucket.Range);

                double certainty = DeviationEstimate.GetComparativeCertainty(bucketCertainty, comparisonBucketCertainty);

                long estimate = (long)(certainty * vBucket.Count);
                if (estimate == 0)
                    return 1;
                else
                    return estimate;
            } else
            {
                return new JoinEstimateEquiDepth().GetBucketEstimate(predicate, bucket, comparisonBucket);
            }
        }
    }
}
