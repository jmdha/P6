using DatabaseConnector;
using Histograms;
using Histograms.Models;
using QueryOptimiser.Exceptions;
using QueryOptimiser.Models;

namespace QueryOptimiser
{
    public class QueryOptimiserEquiDepth : BaseQueryOptimiser
    {
        public QueryOptimiserEquiDepth(IHistogramManager histogramManager) : base(histogramManager)
        {
        }
    }
}
