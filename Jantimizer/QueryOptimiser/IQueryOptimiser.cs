using DatabaseConnector;
using Histograms;
using Histograms.Models;
using QueryOptimiser.Cost.EstimateCalculators;
using QueryOptimiser.Cost.Nodes;
using QueryOptimiser.Models;
using QueryParser.Models;

namespace QueryOptimiser
{
    public interface IQueryOptimiser
    {
        public IHistogramManager HistogramManager { get; }
        public IEstimateCalculator EstimateCalculator { get; }

        public OptimiserResult OptimiseQuery(List<INode> nodes);
    }
}
