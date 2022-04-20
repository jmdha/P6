using DatabaseConnector;
using Histograms;
using Histograms.Models;
using QueryOptimiser.Cost.EstimateCalculators;
using QueryOptimiser.Cost.Nodes;
using QueryOptimiser.Exceptions;
using QueryOptimiser.Helpers;
using QueryOptimiser.Models;
using QueryParser.Models;

namespace QueryOptimiser
{
    public abstract class BaseQueryOptimiser : IQueryOptimiser
    {
        public IHistogramManager HistogramManager { get; }
        public IEstimateCalculator EstimateCalculator { get; internal set; }

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
            
            try
            {
                for (int i = 0; i < nodes.Count; i++)
                {
                    IntermediateTable newTable = EstimateCalculator.EstimateIntermediateTable(nodes[i], intermediateTable);
                    if (intermediateTable.Buckets.Count == 0)
                        intermediateTable = newTable;
                    else
                        intermediateTable = TableHelper.Join(intermediateTable, EstimateCalculator.EstimateIntermediateTable(nodes[i], intermediateTable));
                }
            }
            catch (Exception ex)
            {
                throw new OptimiserErrorLogException(ex, this, nodes);
            }
            

            return new OptimiserResult((ulong)intermediateTable.GetRowEstimate());
        }
    }
}
