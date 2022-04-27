using DatabaseConnector;
using Histograms;
using Histograms.Models;
using QueryOptimiser.Cost.EstimateCalculators;
using QueryOptimiser.Cost.Nodes;
using QueryOptimiser.Exceptions;
using QueryOptimiser.Helpers;
using QueryOptimiser.Models;
using Tools.Models.JsonModels;

namespace QueryOptimiser
{
    public abstract class BaseQueryOptimiser : IQueryOptimiser
    {
        public IHistogramManager HistogramManager { get; }
        public abstract IEstimateCalculator EstimateCalculator { get; internal set; }

        public BaseQueryOptimiser(IHistogramManager histogramManager)
        {
            HistogramManager = histogramManager;
        }

        public OptimiserResult OptimiseQuery(JsonQuery jsonQuery)
        {
            IntermediateTable intermediateTable = new IntermediateTable();
            
            try
            {
                //for (int i = 0; i < jsonQuery.FilterNodes.Count; i++)
                //    intermediateTable = UpdateTable(intermediateTable, EstimateCalculator.EstimateIntermediateTable(jsonQuery.FilterNodes[i], intermediateTable));
                for (int i = 0; i < jsonQuery.JoinNodes.Count; i++)
                    intermediateTable = UpdateTable(intermediateTable, EstimateCalculator.EstimateIntermediateTable(jsonQuery.JoinNodes[i], intermediateTable));
            }
            catch (Exception ex)
            {
                throw new OptimiserErrorLogException(ex, this, jsonQuery.Nodes);
            }
            

            return new OptimiserResult((ulong)intermediateTable.GetRowEstimate(), jsonQuery.Nodes, this.GetType().Name, nameof(HistogramManager), nameof(EstimateCalculator));
        }

        private IntermediateTable UpdateTable(IntermediateTable formerTable, IntermediateTable newTable)
        {
            if (formerTable.Buckets.Count == 0)
                return newTable;
            else
                return TableHelper.Join(formerTable, newTable);
        }
    }
}
