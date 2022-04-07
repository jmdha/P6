
using DatabaseConnector;
using Histograms;
using Histograms.Models;
using QueryOptimiser.Cost.Nodes.EquiDepth;
using QueryOptimiser.Models;
using QueryParser.Models;

namespace QueryOptimiser.Cost.CostCalculators
{
    internal class CostCalculatorEquiDepth : ICostCalculator
    {
        public IHistogramManager HistogramManager { get; set; }

        public CostCalculatorEquiDepth(IHistogramManager histogramManager)
        {
            HistogramManager = histogramManager;
        }

        public CalculationResult CalculateCost(INode node, BucketLimitation limitation)
        {
            if (node is JoinNode joinNode)
            {
                return new JoinCostEquiDepth().CalculateCost(joinNode, HistogramManager, limitation);
            }
            else
            {
                throw new ArgumentException("Non handled node type " + node.ToString());
            }
        }
    }
}
