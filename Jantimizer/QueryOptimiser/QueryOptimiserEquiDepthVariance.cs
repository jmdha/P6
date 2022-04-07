using DatabaseConnector;
using Histograms;
using Histograms.Models;
using QueryOptimiser.Cost.CostCalculators;
using QueryOptimiser.Cost.Nodes;
using QueryOptimiser.Exceptions;
using QueryOptimiser.Models;
using QueryParser.Models;

namespace QueryOptimiser
{
    public class QueryOptimiserEquiDepthVariance : BaseQueryOptimiser
    {
        public QueryOptimiserEquiDepthVariance(IHistogramManager histogramManager) : base(histogramManager)
        {
            CostCalculator = new CostCalculatorEquiDepthVariance(histogramManager);
        }
    }
}
