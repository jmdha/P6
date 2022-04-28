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
using QueryEstimator.Exceptions;

namespace QueryEstimator
{
    public class JsonQueryEstimator : IQueryEstimator<JsonQuery>
    {
        public CrossFilterEstimator FilterEstimator { get; }
        public SimpleFilterEstimator SimpleFilterEstimator { get; }
        public TableAttributeEstimator TableAttributeEstimator { get; }
        public IHistogramManager HistogramManager { get; }

        private Dictionary<TableAttribute, int> _upperBounds;
        private Dictionary<TableAttribute, int> _lowerBounds;
        private Dictionary<string, int> _upperRowBounds;
        private Dictionary<string, int> _lowerRowBounds;
        private TableAttribute _initAttribute = new TableAttribute();

        public JsonQueryEstimator(IHistogramManager histogramManager)
        {
            HistogramManager = histogramManager;
            _upperBounds = new Dictionary<TableAttribute, int>();
            _lowerBounds = new Dictionary<TableAttribute, int>();
            _upperRowBounds = new Dictionary<string, int>();
            _lowerRowBounds = new Dictionary<string, int>();
            FilterEstimator = new CrossFilterEstimator(_upperRowBounds, _lowerRowBounds, histogramManager);
            TableAttributeEstimator = new TableAttributeEstimator(_upperBounds, _lowerBounds, histogramManager);
            SimpleFilterEstimator = new SimpleFilterEstimator(_upperBounds, _lowerBounds, histogramManager);
        }

        public EstimatorResult GetQueryEstimation(JsonQuery query)
        {
            try
            {
                long returnValue = GetIntermediateResults(query).GetTotalEstimation();

                return new EstimatorResult(query, (ulong)returnValue);
            }
            catch (Exception ex)
            {
                throw new EstimatorErrorLogException(ex, query);
            }
        }

        private ISegmentResult GetIntermediateResults(JsonQuery query)
        {
            _upperBounds.Clear();
            _lowerBounds.Clear();

            ISegmentResult result = new ValueTableAttributeResult(0,0, _initAttribute, _initAttribute, 1, ComparisonType.Type.None);

            List<FilterPredicate> baseFilters = new List<FilterPredicate>();
            List<FilterPredicate> crossFilters = new List<FilterPredicate>();
            List<FilterPredicate> simpleFilters = new List<FilterPredicate>();
            List<TableAttributePredicate> predicates = new List<TableAttributePredicate>();
            List<TableAttribute> usedAttributes = new List<TableAttribute>();

            // Scanning
            foreach (var node in query.JoinNodes)
            {
                foreach (var predicate in node.Predicates)
                {
                    if (predicate.LeftAttribute.Attribute != null && predicate.RightAttribute.Attribute != null)
                    {
                        predicates.Add(new TableAttributePredicate(
                            predicate.LeftAttribute.Attribute,
                            predicate.RightAttribute.Attribute,
                            predicate.GetComType()));
                        usedAttributes.Add(predicate.LeftAttribute.Attribute);
                        usedAttributes.Add(predicate.RightAttribute.Attribute);
                    }
                    else if (predicate.LeftAttribute.ConstantValue != null && predicate.RightAttribute.Attribute != null)
                    {
                        baseFilters.Add(new FilterPredicate(
                            predicate.RightAttribute.Attribute,
                            predicate.LeftAttribute.ConstantValue,
                            predicate.GetComType()));
                    }
                    else if (predicate.LeftAttribute.Attribute != null && predicate.RightAttribute.ConstantValue != null)
                    {
                        baseFilters.Add(new FilterPredicate(
                            predicate.LeftAttribute.Attribute,
                            predicate.RightAttribute.ConstantValue,
                            predicate.GetComType()));
                    }
                    else
                        throw new Exception("Impossible predicate detected.");
                }
            }

            foreach(var filter in baseFilters)
            {
                if (usedAttributes.Contains(filter.LeftTable))
                    simpleFilters.Add(filter);
                else
                    crossFilters.Add(filter);
            }

            // Simple filters
            foreach (var filter in simpleFilters)
            {
                SimpleFilterEstimator.GetEstimationResult(
                    result,
                    filter.LeftTable,
                    filter.ConstantValue,
                    filter.ComType);
            }

            // Cross filters
            foreach (var filter in crossFilters)
            {
                FilterEstimator.GetEstimationResult(
                    result,
                    filter.LeftTable,
                    filter.ConstantValue,
                    filter.ComType);
            }

            // Predicates
            foreach (var predicate in predicates)
            {
                result = TableAttributeEstimator.GetEstimationResult(
                    result,
                    predicate.LeftTable,
                    predicate.RightTable,
                    predicate.ComType);
            }

            CalculateBounds(result);

            return result;
        }

        private ISegmentResult CalculateBounds(ISegmentResult current)
        {
            if (current is SegmentResult seg)
            {
                seg.Left = CalculateBounds(seg.Left);
                seg.Right = CalculateBounds(seg.Right);
                return seg;
            }
            if (current is ValueTableAttributeResult res)
            {
                if (res.TableA.Equals(_initAttribute))
                    return current;
                if (res.TableALowerBound != _lowerBounds[res.TableA] || res.TableAUpperBound != _upperBounds[res.TableA])
                {
                    return TableAttributeEstimator.GetEstimationResult(
                        res,
                        res.TableA,
                        res.TableB,
                        res.ComType);
                }
                else
                    return current;
            }
            if (current is ValueFilterResult fil)
            {
                if (fil.TableA.Equals(_initAttribute))
                    return current;
                if (fil.TableALowerBound != _lowerBounds[fil.TableA] || fil.TableAUpperBound != _upperBounds[fil.TableA])
                {
                    return FilterEstimator.GetEstimationResult(
                        fil,
                        fil.TableA,
                        fil.ConstantValue,
                        fil.ComType);
                }
                else
                    return current;
            }
            throw new Exception("Bad node type!");
        }
    }
}
