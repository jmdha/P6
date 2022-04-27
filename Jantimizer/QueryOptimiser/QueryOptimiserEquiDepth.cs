using DatabaseConnector;
using Histograms;
using Histograms.Models;
using QueryOptimiser.Cost.EstimateCalculators;
using QueryOptimiser.Cost.Nodes;
using QueryOptimiser.Exceptions;
using QueryOptimiser.Models;

namespace QueryOptimiser
{
    public class QueryOptimiserEquiDepth : BaseQueryOptimiser
    {
        public override IEstimateCalculator EstimateCalculator { get; internal set; }
        public QueryOptimiserEquiDepth(IHistogramManager histogramManager) : base(histogramManager)
        {
            EstimateCalculator = new EstimateCalculatorEquiDepth(histogramManager);
        }
    }
}
