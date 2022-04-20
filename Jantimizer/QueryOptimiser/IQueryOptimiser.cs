using DatabaseConnector;
using Histograms;
using Histograms.Models;
using QueryOptimiser.Cost.EstimateCalculators;
using QueryOptimiser.Cost.Nodes;
using QueryOptimiser.Models;
using QueryParser.Models;
using QueryParser.QueryParsers;

namespace QueryOptimiser
{
    public interface IQueryOptimiser
    {
        public IHistogramManager HistogramManager { get; }
        public IEstimateCalculator EstimateCalculator { get; }

        /// <summary>
        /// Reorders a querys join order according to the cost of each join operation
        /// </summary>
        /// <returns></returns>
        public OptimiserResult OptimiseQuery(ParserResult result);
    }
}
