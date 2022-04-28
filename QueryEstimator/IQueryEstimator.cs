using QueryEstimator.Models;
using Tools.Models.JsonModels;

namespace QueryEstimator
{
    public interface IQueryEstimator<T> where T : notnull
    {
        public EstimatorResult GetQueryEstimation(T query);
    }
}