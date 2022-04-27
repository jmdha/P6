using DatabaseConnector;
using Histograms;
using Histograms.Models;
using QueryOptimiser.Cost.EstimateCalculators;
using QueryOptimiser.Cost.Nodes;
using QueryOptimiser.Exceptions;
using QueryOptimiser.Models;

namespace QueryOptimiser
{
    public class QueryOptimiserEquiDepthVariance : BaseQueryOptimiser
    {
        public override IEstimateCalculator EstimateCalculator { get; internal set; }
        public QueryOptimiserEquiDepthVariance(IHistogramManager histogramManager) : base(histogramManager)
        {
            EstimateCalculator = new EstimateCalculatorVariance(histogramManager);
        }
    }
}
