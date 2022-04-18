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

[assembly: InternalsVisibleTo("QueryOptimiserTest")]

namespace QueryOptimiser.Cost.Nodes
{
    internal class JoinEstimateEquiDepth : BaseJoinCost
    {
        public override long GetBucketEstimate(ComparisonType.Type predicate, IHistogramBucket bucket, IHistogramBucket comparisonBucket)
        {
            return bucket.Count;
        }
    }
}
