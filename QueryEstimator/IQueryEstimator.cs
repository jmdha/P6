using Histograms;
using QueryEstimator.Mergers;
using QueryEstimator.Models;
using QueryEstimator.PredicateEstimators;
using Tools.Models.JsonModels;

namespace QueryEstimator
{
    public interface IQueryEstimator<T> 
        where T : notnull
    {
        public IHistogramManager HistogramManager { get; }
        public EstimatorResult GetQueryEstimation(T query);
    }
}