using DatabaseConnector;
using Histograms;
using Histograms.Models;
using QueryOptimiser.Exceptions;
using QueryOptimiser.Models;
using Tools.Models.JsonModels;

namespace QueryOptimiser
{
    public abstract class BaseQueryOptimiser : IQueryOptimiser
    {
        public IHistogramManager HistogramManager { get; }

        public BaseQueryOptimiser(IHistogramManager histogramManager)
        {
            HistogramManager = histogramManager;
        }

        public OptimiserResult OptimiseQuery(JsonQuery jsonQuery)
        {
            return new OptimiserResult(1, jsonQuery.Nodes, this.GetType().Name, nameof(HistogramManager), "");
        }
    }
}
