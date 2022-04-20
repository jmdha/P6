
using DatabaseConnector;
using Histograms;
using Histograms.Models;
using QueryOptimiser.Cost.Nodes;
using QueryOptimiser.Models;
using QueryParser.Models;

namespace QueryOptimiser.Cost.EstimateCalculators
{
    internal class EstimateCalculatorEquiDepth : BaseEstimateCalculator
    {
        public override INodeCost<JoinNode> NodeCostCalculator { get; set; }

        public EstimateCalculatorEquiDepth(IHistogramManager manager) : base(manager) {
            NodeCostCalculator = new JoinEstimateEquiDepth();
        }
    }
}
