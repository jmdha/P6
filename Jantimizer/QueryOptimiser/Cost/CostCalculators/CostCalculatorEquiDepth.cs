
using DatabaseConnector;
using Histograms;
using QueryOptimiser.Cost.Nodes.EquiDepth;
using QueryParser.Models;

namespace QueryOptimiser.Cost.CostCalculators
{
    public class CostCalculatorEquiDepth : ICostCalculator<HistogramEquiDepth, IDbConnector>
    {
        public IHistogramManager<HistogramEquiDepth, IDbConnector> HistogramManager { get; set; }

        public CostCalculatorEquiDepth(IHistogramManager<HistogramEquiDepth, IDbConnector> histogramManager)
        {
            HistogramManager = histogramManager;
        }

        public int CalculateCost(INode node)
        {
            if (node is JoinNode joinNode)
            {
                return new JoinCost().CalculateCost(joinNode, HistogramManager);
            }
            else
            {
                throw new ArgumentException("Non handled node type " + node.ToString());
            }
        }
    }
}
