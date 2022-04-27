using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Histograms;
using Histograms.Models;
using DatabaseConnector;
using System.Runtime.CompilerServices;
using Tools.Models.JsonModels;

namespace QueryOptimiser.Cost.Nodes
{
    public class JoinEstimateEquiDepth : IJoinEstimate
    {
        public long GetBucketEstimate(ComparisonType.Type predicate, IHistogramBucket bucket, IHistogramBucket comparisonBucket)
        {
            return bucket.Count;
        }
    }
}
