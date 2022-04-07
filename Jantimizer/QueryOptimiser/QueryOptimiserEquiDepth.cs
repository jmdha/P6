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
    public class QueryOptimiserEquiDepth : BaseQueryOptimiser
    {
        public QueryOptimiserEquiDepth(IHistogramManager histogramManager) : base(histogramManager)
        {
            CostCalculator = new CostCalculatorEquiDepth(histogramManager);
        }
    }
}
