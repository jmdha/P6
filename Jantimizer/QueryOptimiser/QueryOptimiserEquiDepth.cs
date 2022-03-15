
using DatabaseConnector;
using Histograms;
using QueryOptimiser.Cost.CostCalculators;
using QueryOptimiser.Cost.Nodes;
using QueryOptimiser.QueryGenerators;
using QueryParser.Models;

namespace QueryOptimiser
{
    public class QueryOptimiserEquiDepth : IQueryOptimiser<HistogramEquiDepth, IDbConnector>
    {
        public IQueryGenerator QueryGenerator { get; set; }
        public IHistogramManager<HistogramEquiDepth, IDbConnector> HistogramManager { get; set; }
        public ICostCalculator<HistogramEquiDepth, IDbConnector> CostCalculator { get; set; }

        public QueryOptimiserEquiDepth(IQueryGenerator queryGenerator, IHistogramManager<HistogramEquiDepth, IDbConnector> histogramManager)
        {
            QueryGenerator = queryGenerator;
            HistogramManager = histogramManager;
            CostCalculator = new CostCalculatorEquiDepth(histogramManager);
        }

        /// <summary>
        /// Reorders a querys join order according to the cost of each join operation
        /// </summary>
        /// <returns></returns>
        public string OptimiseQuery(List<INode> nodes)
        {
            List<ValuedNode> valuedNodes = CalculateNodeCost(nodes);

            return QueryGenerator.GenerateQuery(valuedNodes);
        }

        public ulong OptimiseQueryCardinality(List<INode> nodes)
        {
            List<ValuedNode> valuedNodes = CalculateNodeCost(nodes);
            if (valuedNodes.Count == 0)
                return 0;
            ulong expCardinality = 0;
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
                int cost = CostCalculator.CalculateCost(node);
                valuedNodes.Add(new ValuedNode(cost, node));
            }
            return valuedNodes;
        }


    }
}
