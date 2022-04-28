using Histograms;
using Histograms.Models;
using QueryEstimator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.Models.JsonModels;
using Tools.Helpers;
using QueryEstimator.PredicateEstimators;
using QueryEstimator.Mergers;
using QueryEstimator.Exceptions;

namespace QueryEstimator
{
    public class JsonQueryEstimator : IJsonQueryEstimator<
        JsonQuery,
        Dictionary<TableAttribute, int>,
        Dictionary<TableAttribute, List<ISegmentResult>>,
        TableAttribute,
        IComparable,
        TableAttribute,
        TableAttribute,
        List<ISegmentResult>>
    {
        public IPredicateEstimator<
            Dictionary<TableAttribute, int>, 
            Dictionary<TableAttribute, List<ISegmentResult>>, 
            TableAttribute, 
            IComparable> FilterEstimator { get; }
        public IPredicateEstimator<
            Dictionary<TableAttribute, int>,
            Dictionary<TableAttribute, List<ISegmentResult>>,
            TableAttribute,
            TableAttribute> TableAttributeEstimator { get; }
        public IMerger<
            Dictionary<TableAttribute, int>, 
            List<ISegmentResult>, 
            Dictionary<TableAttribute, List<ISegmentResult>>> SegmentMerger { get; }
        public IHistogramManager HistogramManager { get; }

        private Dictionary<TableAttribute, int> _upperBounds;
        private Dictionary<TableAttribute, int> _lowerBounds;

        public JsonQueryEstimator(IHistogramManager histogramManager)
        {
            HistogramManager = histogramManager;
            _upperBounds = new Dictionary<TableAttribute, int>();
            _lowerBounds = new Dictionary<TableAttribute, int>();
            FilterEstimator = new FilterEstimator(_upperBounds, _lowerBounds);
            TableAttributeEstimator = new TableAttributeEstimator(_upperBounds, _lowerBounds);
            SegmentMerger = new SegmentListMerger(_upperBounds, _lowerBounds);
        }

        public EstimatorResult GetQueryEstimation(JsonQuery query)
        {
            try
            {
                long returnValue = StitchResults(GetIntermediateResults(query));

                return new EstimatorResult(query, (ulong)returnValue);
            }
            catch (Exception ex)
            {
                throw new EstimatorErrorLogException(ex, query);
            }
        }

        private long StitchResults(Dictionary<TableAttribute, List<ISegmentResult>> dict)
        {
            long returnValue = 0;

            foreach (var value in SegmentMerger.Stitch(dict))
            {
                returnValue += value.GetTotalEstimation();
            }

            return returnValue;
        }

        private Dictionary<TableAttribute, List<ISegmentResult>> GetIntermediateResults(JsonQuery query)
        {
            _upperBounds.Clear();
            _lowerBounds.Clear();

            Dictionary<TableAttribute, List<ISegmentResult>> intermediateResults = new Dictionary<TableAttribute, List<ISegmentResult>>();
            foreach (var node in query.JoinNodes)
            {
                foreach (var predicate in node.Predicates)
                {
                    if (predicate.LeftAttribute.Attribute != null && predicate.RightAttribute.Attribute != null)
                    {
                        TableAttributeEstimator.GetEstimationResult(
                            intermediateResults,
                            predicate.LeftAttribute.Attribute,
                            predicate.RightAttribute.Attribute,
                            predicate.GetComType());
                    }
                    else if (predicate.LeftAttribute.ConstantValue != null && predicate.RightAttribute.Attribute != null)
                    {
                        if (predicate.GetComType() == ComparisonType.Type.Less)
                            FilterEstimator.GetEstimationResult(
                                intermediateResults,
                                predicate.RightAttribute.Attribute,
                                predicate.LeftAttribute.ConstantValue,
                                ComparisonType.Type.More);
                        if (predicate.GetComType() == ComparisonType.Type.More)
                            FilterEstimator.GetEstimationResult(
                                intermediateResults,
                                predicate.RightAttribute.Attribute,
                                predicate.LeftAttribute.ConstantValue,
                                ComparisonType.Type.Less);
                    }
                    else if (predicate.LeftAttribute.Attribute != null && predicate.RightAttribute.ConstantValue != null)
                    {
                        if (predicate.GetComType() == ComparisonType.Type.Less)
                            FilterEstimator.GetEstimationResult(
                                intermediateResults,
                                predicate.LeftAttribute.Attribute,
                                predicate.RightAttribute.ConstantValue,
                                ComparisonType.Type.More);
                        if (predicate.GetComType() == ComparisonType.Type.More)
                            FilterEstimator.GetEstimationResult(
                                intermediateResults,
                                predicate.LeftAttribute.Attribute,
                                predicate.RightAttribute.ConstantValue,
                                ComparisonType.Type.Less);
                    }
                    else
                        throw new Exception("Impossible predicate detected.");
                }
            }
            return intermediateResults;
        }
    }
}
