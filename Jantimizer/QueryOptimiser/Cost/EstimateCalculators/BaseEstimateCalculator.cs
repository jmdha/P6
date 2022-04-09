using Histograms;
using QueryOptimiser.Cost.Nodes;
using QueryOptimiser.Models;
using QueryParser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryOptimiser.Cost.EstimateCalculators
{
    public abstract class BaseEstimateCalculator : IEstimateCalculator
    {
        public IHistogramManager HistogramManager { get; set; }

        internal abstract INodeCost<JoinNode> JoinCost { get; set; }

        internal BaseEstimateCalculator(IHistogramManager manager)
        {
            HistogramManager = manager;
        }

        public IntermediateTable EstimateIntermediateTable(INode node, IntermediateTable intermediateTable)
        {
            if (node is JoinNode joinNode)
            {
                return JoinCost.GetMatches(joinNode, HistogramManager, intermediateTable);
            }
            else
            {
                throw new ArgumentException("Non handled node type " + node.ToString());
            }
        }
    }
}
