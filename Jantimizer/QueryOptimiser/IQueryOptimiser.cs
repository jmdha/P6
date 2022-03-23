using DatabaseConnector;
using Histograms;
using QueryOptimiser.Cost.CostCalculators;
using QueryOptimiser.Cost.Nodes;
using QueryParser.Models;

namespace QueryOptimiser
{
    public interface IQueryOptimiser<HistogramType, ConnectorType>
        where HistogramType : IHistogram
        where ConnectorType : IDbConnector
    {
        public string Name { get; }
        public string Version { get; }

        public IHistogramManager<HistogramType, ConnectorType> HistogramManager { get; }
        public ICostCalculator<HistogramType, ConnectorType> CostCalculator { get; }

        /// <summary>
        /// Reorders a querys join order according to the cost of each join operation
        /// </summary>
        /// <returns></returns>
        public List<ValuedNode> OptimiseQuery(List<INode> nodes);

        public ulong OptimiseQueryCardinality(List<INode> nodes);

        /// <summary>
        /// Calculates worst case cost of each join operation
        /// </summary>
        /// <returns></returns>
        public List<ValuedNode> CalculateNodeCost(List<INode> nodes);
    }
}
