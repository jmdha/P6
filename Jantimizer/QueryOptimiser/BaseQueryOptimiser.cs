using DatabaseConnector;
using Histograms;
using Histograms.Models;
using QueryOptimiser.Cost.EstimateCalculators;
using QueryOptimiser.Cost.Nodes;
using QueryOptimiser.Models;
using QueryParser.Models;

namespace QueryOptimiser
{
    public partial class BaseQueryOptimiser : IQueryOptimiser
    {
        public IHistogramManager HistogramManager { get; set; }
        public IEstimateCalculator EstimateCalculator { get; set; }

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
            IntermediateTable intermediateTable = new IntermediateTable();
            
            List<ValuedNode> valuedNodes = new List<ValuedNode>();
            for (int i = 0; i < nodes.Count; i++)
            {
                CalculationResult result = EstimateCalculator.EstimateIntermediateTable(nodes[i], intermediateTable);
                valuedNodes.Add(new ValuedNode(result.Estimate, nodes[i]));
                if (result.Table != null)
                {
                    intermediateTable = result.Table;
                }
            }
             
            if (valuedNodes.Count == 0)
                return new OptimiserResult(0, new List<ValuedNode>());
            ulong expCardinality = 1;
            foreach (ValuedNode node in valuedNodes)
                expCardinality *= (ulong)node.Cost;

            return new OptimiserResult(expCardinality, valuedNodes);
        }
    }
}
