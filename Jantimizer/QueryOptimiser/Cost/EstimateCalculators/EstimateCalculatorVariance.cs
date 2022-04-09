
using DatabaseConnector;
using Histograms;
using Histograms.Models;
using QueryOptimiser.Cost.Nodes;
using QueryOptimiser.Cost.Nodes.EquiDepthVariance;
using QueryOptimiser.Models;
using QueryParser.Models;

namespace QueryOptimiser.Cost.EstimateCalculators
{
    internal class EstimateCalculatorVariance : BaseEstimateCalculator
    {
        internal override INodeCost<JoinNode> JoinCost { get; set; }

        public EstimateCalculatorVariance(IHistogramManager manager) : base(manager) {
            JoinCost = new JoinCostEquiDepthVariance();
        }
    }
}
