
using DatabaseConnector;
using Histograms;
using Histograms.Models;
using QueryOptimiser.Cost.Nodes.EquiDepthVariance;
using QueryParser.Models;

namespace QueryOptimiser.Cost.CostCalculators
{
    public class CostCalculatorEquiDepthVariance : ICostCalculator
    {
        public IHistogramManager HistogramManager { get; set; }

        public CostCalculatorEquiDepthVariance(IHistogramManager histogramManager)
        {
            HistogramManager = histogramManager;
        }

        public long CalculateCost(INode node)
        {
            if (node is JoinNode joinNode)
            {
                return new JoinCostEquiDepthVariance().CalculateCost(joinNode, HistogramManager.GetHistogram("", "").Buckets, HistogramManager.GetHistogram("", "").Buckets);
            }
            else
            {
                throw new ArgumentException("Non handled node type " + node.ToString());
            }
        }
    }
}
