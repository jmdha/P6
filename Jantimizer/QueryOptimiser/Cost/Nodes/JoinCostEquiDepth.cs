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
    public class JoinCostEquiDepth : BaseJoinCost
    {
        protected override long CalculateCost(ComparisonType.Type predicate, HistogramBucket leftBucket, IHistogramBucket rightBucket)
        {
            return leftBucket.Count * rightBucket.Count;
        }
    }
}
