using DatabaseConnector;
using Histograms;
using QueryOptimiser.Cost.CostCalculators;
using QueryOptimiser.Cost.Nodes;
using QueryOptimiser.Models;
using QueryParser.Models;

namespace QueryOptimiser
{
    public interface IQueryOptimiser<HistogramType, ConnectorType>
        where HistogramType : IHistogram
        where ConnectorType : IDbConnector
    {
        public IHistogramManager<HistogramType, ConnectorType> HistogramManager { get; }
        public ICostCalculator<HistogramType, ConnectorType> CostCalculator { get; }

        /// <summary>
        /// Reorders a querys join order according to the cost of each join operation
        /// </summary>
        /// <returns></returns>
        public OptimiserResult OptimiseQuery(List<INode> nodes);
    }
}
