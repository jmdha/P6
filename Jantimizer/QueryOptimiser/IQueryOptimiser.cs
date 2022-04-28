using DatabaseConnector;
using Histograms;
using Histograms.Models;
using QueryOptimiser.Models;
using Tools.Models.JsonModels;

namespace QueryOptimiser
{
    public interface IQueryOptimiser
    {
        public IHistogramManager HistogramManager { get; }

        public OptimiserResult OptimiseQuery(JsonQuery jsonQuery);
    }
}
