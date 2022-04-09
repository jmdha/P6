
using DatabaseConnector;
using Histograms;
using Histograms.Models;
using QueryOptimiser.Cost.Nodes;
using QueryOptimiser.Cost.Nodes.EquiDepth;
using QueryOptimiser.Models;
using QueryParser.Models;

namespace QueryOptimiser.Cost.EstimateCalculators
{
    internal class EstimateCalculatorEquiDepth : BaseEstimateCalculator
    {
        internal override INodeCost<JoinNode> JoinCost { get; set; }

        public EstimateCalculatorEquiDepth(IHistogramManager manager) : base(manager) {
            JoinCost = new JoinCostEquiDepth();
        }
    }
}
