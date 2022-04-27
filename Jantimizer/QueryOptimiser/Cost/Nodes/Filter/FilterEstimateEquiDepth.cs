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
    public class FilterEstimateEquiDepth : IFilterEstimate
    {
        public long GetBucketEstimate(ComparisonType.Type comparisonType, IComparable constant, IHistogramBucket bucket)
        {
            return bucket.Count;
        }
    }
}
