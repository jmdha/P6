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
            BucketDictionary matchedBuckets = new BucketDictionary();
            
            List<ValuedNode> valuedNodes = new List<ValuedNode>();
            for (int i = 0; i < nodes.Count; i++)
            {
                CalculationResult result = CalculateNodeCost(nodes[i]);
                valuedNodes.Add(new ValuedNode(result.Estimate, nodes[i]));
            }
             
            if (valuedNodes.Count == 0)
                return new OptimiserResult(0, new List<ValuedNode>());
            ulong expCardinality = 1;
            foreach (ValuedNode node in valuedNodes)
                expCardinality *= (ulong)node.Cost;

            return new OptimiserResult(expCardinality, valuedNodes);
        }

        internal CalculationResult CalculateNodeCost(INode node)
        {
            return CostCalculator.CalculateCost(node);
        }
    }
}
