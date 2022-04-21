using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QueryParser.Models;
using Histograms;
using Histograms.Models;
using DatabaseConnector;
using System.Runtime.CompilerServices;

namespace QueryOptimiser.Cost.Nodes
{
    public class JoinEstimateEquiDepth : INodeCost<JoinNode>
    {
        public long GetBucketEstimate(ComparisonType.Type predicate, IHistogramBucket bucket, IHistogramBucket comparisonBucket)
        {
            return bucket.Count;
        }
    }
}
