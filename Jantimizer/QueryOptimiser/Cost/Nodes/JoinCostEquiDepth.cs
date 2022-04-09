using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QueryParser.Models;
using Histograms;
using Histograms.Models;
using DatabaseConnector;

namespace QueryOptimiser.Cost.Nodes.EquiDepth
{
    internal class JoinCostEquiDepth : BaseJoinCost
    {
        public override long GetCombinedEstimate(ComparisonType.Type predicate, IHistogramBucket leftBucket, IHistogramBucket rightBucket)
        {
            return leftBucket.Count * rightBucket.Count;
        }
    }
}
