
using DatabaseConnector;
using Histograms;
using Histograms.Models;
using QueryOptimiser.Cost.Nodes.EquiDepthVariance;
using QueryOptimiser.Models;
using QueryParser.Models;

namespace QueryOptimiser.Cost.CostCalculators
{
    internal class CostCalculatorEquiDepthVariance : ICostCalculator
    {
        public IHistogramManager HistogramManager { get; set; }

        public CostCalculatorEquiDepthVariance(IHistogramManager histogramManager)
        {
            HistogramManager = histogramManager;
        }

        public CalculationResult CalculateCost(INode node)
        {
            if (node is JoinNode joinNode)
            {
                return new JoinCostEquiDepthVariance().CalculateCost(joinNode, HistogramManager);
            }
            else
            {
                throw new ArgumentException("Non handled node type " + node.ToString());
            }
        }
    }
}
