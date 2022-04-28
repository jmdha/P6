using Histograms;
using QueryEstimator.Models;
using QueryEstimator.PredicateEstimators;
using Tools.Models.JsonModels;

namespace QueryEstimator
{
    public interface IJsonQueryEstimator<T, TBounds, TFilterLeft, TFilterRight, TAttributeLeft, TAttributeRight> : IQueryEstimator<T>
        where T : notnull
    {
        public IPredicateEstimator<TBounds, TFilterLeft, TFilterRight> FilterEstimator { get; }
        public IPredicateEstimator<TBounds, TAttributeLeft, TAttributeRight> TableAttributeEstimator { get; }
    }
}