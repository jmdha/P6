using DatabaseConnector;
using Histograms;
using Histograms.Models;
using QueryOptimiser.Cost.CostCalculators;
using QueryOptimiser.Cost.Nodes;
using QueryOptimiser.Models;
using QueryParser.Models;

namespace QueryOptimiser
{
    public partial class BaseQueryOptimiser : IQueryOptimiser
    {
        public IHistogramManager HistogramManager { get; set; }
        public ICostCalculator CostCalculator { get; set; }

        public BaseQueryOptimiser(IHistogramManager histogramManager)
        {
            HistogramManager = histogramManager;
        }

        /// <summary>
        /// Reorders a querys join order according to the cost of each join operation
        /// </summary>
        /// <returns></returns>
        public OptimiserResult OptimiseQuery(List<INode> nodes)
        {
            List<ValuedNode> valuedNodes = CalculateNodeCost(nodes).OrderByDescending(x => -x.Cost).ToList();
            if (valuedNodes.Count == 0)
                return new OptimiserResult(0, new List<ValuedNode>());
            ulong expCardinality = 1;
            foreach (ValuedNode node in valuedNodes)
                expCardinality *= (ulong)node.Cost;

            return new OptimiserResult(expCardinality, valuedNodes);
        }

        /// <summary>
        /// Calculates worst case cost of each node
        /// </summary>
        /// <returns></returns>
        internal List<ValuedNode> CalculateNodeCost(List<INode> nodes)
        {
            List<ValuedNode> valuedNodes = new List<ValuedNode>();
            foreach (var node in nodes)
            {
                long cost = CostCalculator.CalculateCost(node);
                valuedNodes.Add(new ValuedNode(cost, node));
            }
            return valuedNodes;
        }

        /// <summary>
        /// Calculates worst case cost of a node
        /// </summary>
        /// <returns></returns>
        internal ValuedNode CalculateNodeCost(INode node)
        {
            long cost = CostCalculator.CalculateCost(node);
            return new ValuedNode(cost, node);
        }
    }
}
