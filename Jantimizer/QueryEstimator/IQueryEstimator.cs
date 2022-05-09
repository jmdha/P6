using QueryEstimator.Models;
using QueryEstimator.PredicateEstimators;
using Milestoner;
using Tools.Models.JsonModels;
using QueryEstimator.Models.BoundResults;

namespace QueryEstimator
{
    public interface IQueryEstimator<T> 
        where T : notnull
    {
        public IMilestoner Milestoner { get; }
        public EstimatorResult GetQueryEstimation(T query);
        public Dictionary<int, List<IPredicateBoundResult<TableAttribute>>> TableAttributeBounds { get; }
        public Dictionary<int, List<IPredicateBoundResult<IComparable>>> FilterBounds { get; }
        public ISegmentResult? ResultChain { get; }
    }
}