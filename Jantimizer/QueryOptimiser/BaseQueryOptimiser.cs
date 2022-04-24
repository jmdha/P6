using DatabaseConnector;
using Histograms;
using Histograms.Models;
using QueryOptimiser.Cost.EstimateCalculators;
using QueryOptimiser.Cost.Nodes;
using QueryOptimiser.Exceptions;
using QueryOptimiser.Helpers;
using QueryOptimiser.Models;
using QueryParser.Models;
using QueryParser.QueryParsers;

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

        public OptimiserResult OptimiseQuery(ParserResult parserResult)
        {
            IntermediateTable intermediateTable = new IntermediateTable();
            
            try
            {
                for (int i = 0; i < parserResult.Filters.Count; i++)
                    intermediateTable = UpdateTable(intermediateTable, EstimateCalculator.EstimateIntermediateTable(parserResult.Filters[i], intermediateTable));
                for (int i = 0; i < parserResult.Joins.Count; i++)
                    intermediateTable = UpdateTable(intermediateTable, EstimateCalculator.EstimateIntermediateTable(parserResult.Joins[i], intermediateTable));
            }
            catch (Exception ex)
            {
                throw new OptimiserErrorLogException(ex, this, parserResult.Nodes);
            }
            

            return new OptimiserResult((ulong)intermediateTable.GetRowEstimate(), parserResult.Nodes, this.GetType().Name, nameof(HistogramManager), nameof(EstimateCalculator));
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
