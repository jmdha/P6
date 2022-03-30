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
        protected override long CalculateCost(ComparisonType.Type predicate, IHistogramBucket[] leftBuckets, IHistogramBucket[] rightBuckets)
        {
            long leftSum = 0;
            long rightSum = 0;

            for (int i = 0; i < leftBuckets.Length; i++)
                leftSum += leftBuckets[i].Count;
            for (int i = 0; i < rightBuckets.Length; i++)
                rightSum += rightBuckets[i].Count;
            return leftSum * rightSum;
        }
    }
}
