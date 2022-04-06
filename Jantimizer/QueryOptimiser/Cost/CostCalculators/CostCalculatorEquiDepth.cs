
using DatabaseConnector;
using Histograms;
using Histograms.Models;
using QueryOptimiser.Cost.Nodes.EquiDepth;
using QueryParser.Models;

namespace QueryOptimiser.Cost.CostCalculators
{
    public class CostCalculatorEquiDepth : ICostCalculator
    {
        public IHistogramManager HistogramManager { get; set; }

        public CostCalculatorEquiDepth(IHistogramManager histogramManager)
        {
            HistogramManager = histogramManager;
        }

        public long CalculateCost(INode node)
        {
            if (node is JoinNode joinNode)
            {
                return new JoinCostEquiDepth().CalculateCost(joinNode, HistogramManager);
            }
            else
            {
                throw new ArgumentException("Non handled node type " + node.ToString());
            }
        }
    }
}
