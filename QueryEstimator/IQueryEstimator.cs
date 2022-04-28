using Histograms;
using QueryEstimator.Models;
using Tools.Models.JsonModels;

namespace QueryEstimator
{
    public interface IQueryEstimator<T> where T : notnull
    {
        public IHistogramManager HistogramManager { get; }
        public EstimatorResult GetQueryEstimation(T query);
    }
}