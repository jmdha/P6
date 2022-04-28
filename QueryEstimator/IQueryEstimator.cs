using Histograms;
using QueryEstimator.Mergers;
using QueryEstimator.Models;
using QueryEstimator.PredicateEstimators;
using Tools.Models.JsonModels;

namespace QueryEstimator
{
    public interface IQueryEstimator<T, TBounds, TIntermediate, TFilterLeft, TFilterRight, TAttributeLeft, TAttributeRight, TMergeList> 
        where T : notnull
    {
        public IPredicateEstimator<TBounds, TIntermediate, TFilterLeft, TFilterRight> FilterEstimator { get; }
        public IPredicateEstimator<TBounds, TIntermediate, TAttributeLeft, TAttributeRight> TableAttributeEstimator { get; }
        public IMerger<TBounds, TMergeList, TIntermediate> SegmentMerger { get; }

        public IHistogramManager HistogramManager { get; }
        public EstimatorResult GetQueryEstimation(T query);
    }
}