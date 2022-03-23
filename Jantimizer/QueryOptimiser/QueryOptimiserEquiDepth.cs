using DatabaseConnector;
using Histograms;
using QueryOptimiser.Cost.CostCalculators;
using QueryOptimiser.Cost.Nodes;
using QueryParser.Models;

namespace QueryOptimiser
{
    public class QueryOptimiserEquiDepth : IQueryOptimiser<IHistogram, IDbConnector>
    {
        public string Name { get; } = "Jantimiser";
        public string Version { get; } = "1.0";

        public IHistogramManager<IHistogram, IDbConnector> HistogramManager { get; set; }
        public ICostCalculator<IHistogram, IDbConnector> CostCalculator { get; set; }

        public QueryOptimiserEquiDepth(IHistogramManager<IHistogram, IDbConnector> histogramManager)
        {
            HistogramManager = histogramManager;
            CostCalculator = new CostCalculatorEquiDepth(histogramManager);
        }

        /// <summary>
        /// Reorders a querys join order according to the cost of each join operation
        /// </summary>
        /// <returns></returns>
        public List<ValuedNode> OptimiseQuery(List<INode> nodes)
        {
            return CalculateNodeCost(nodes).OrderByDescending(x => -x.Cost).ToList();
        }

        public ulong OptimiseQueryCardinality(List<INode> nodes)
        {
            List<ValuedNode> valuedNodes = CalculateNodeCost(nodes);
            if (valuedNodes.Count == 0)
                return 0;
            ulong expCardinality = 1;
            foreach (ValuedNode node in valuedNodes)
                expCardinality *= (ulong)node.Cost;

            return expCardinality;
        }

        /// <summary>
        /// Calculates worst case cost of each join operation
        /// </summary>
        /// <returns></returns>
        public List<ValuedNode> CalculateNodeCost(List<INode> nodes)
        {
            List<ValuedNode> valuedNodes = new List<ValuedNode>();
            foreach (var node in nodes)
            {
                long cost = CostCalculator.CalculateCost(node);
                valuedNodes.Add(new ValuedNode(cost, node));
            }
            return valuedNodes;
        }


    }
}
