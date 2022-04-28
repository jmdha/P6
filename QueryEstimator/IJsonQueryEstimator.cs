using Histograms;
using QueryEstimator.Mergers;
using QueryEstimator.Models;
using QueryEstimator.PredicateEstimators;
using Tools.Models.JsonModels;

namespace QueryEstimator
{
    public interface IJsonQueryEstimator<T, TBounds, TIntermediate, TFilterLeft, TFilterRight, TAttributeLeft, TAttributeRight, TMergeList> : IQueryEstimator<T>
        where T : notnull
    {
        public IPredicateEstimator<TBounds, TIntermediate, TFilterLeft, TFilterRight> FilterEstimator { get; }
        public IPredicateEstimator<TBounds, TIntermediate, TAttributeLeft, TAttributeRight> TableAttributeEstimator { get; }
        public IMerger<TBounds, TMergeList, TIntermediate> SegmentMerger { get; }
    }
}