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
    public class JsonQueryEstimator : IJsonQueryEstimator<
        JsonQuery,
        Dictionary<TableAttribute, int>,
        TableAttribute,
        IComparable,
        TableAttribute,
        TableAttribute>
    {
        public IPredicateEstimator<
            Dictionary<TableAttribute, int>, 
            TableAttribute, 
            IComparable> FilterEstimator { get; }
        public IPredicateEstimator<
            Dictionary<TableAttribute, int>,
            TableAttribute,
            TableAttribute> TableAttributeEstimator { get; }
        public IHistogramManager HistogramManager { get; }

        private Dictionary<TableAttribute, int> _upperBounds;
        private Dictionary<TableAttribute, int> _lowerBounds;
        private TableAttribute _initAttribute = new TableAttribute();

        public JsonQueryEstimator(IHistogramManager histogramManager)
        {
            HistogramManager = histogramManager;
            _upperBounds = new Dictionary<TableAttribute, int>();
            _lowerBounds = new Dictionary<TableAttribute, int>();
            FilterEstimator = new FilterEstimator(_upperBounds, _lowerBounds, histogramManager);
            TableAttributeEstimator = new TableAttributeEstimator(_upperBounds, _lowerBounds, histogramManager);
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

            foreach (var node in query.JoinNodes)
            {
                foreach (var predicate in node.Predicates)
                {
                    if (predicate.LeftAttribute.Attribute != null && predicate.RightAttribute.Attribute != null)
                    {
                        result = TableAttributeEstimator.GetEstimationResult(
                            result,
                            predicate.LeftAttribute.Attribute,
                            predicate.RightAttribute.Attribute,
                            predicate.GetComType());
                    }
                    else if (predicate.LeftAttribute.ConstantValue != null && predicate.RightAttribute.Attribute != null)
                    {
                        result = FilterEstimator.GetEstimationResult(
                            result,
                            predicate.RightAttribute.Attribute,
                            predicate.LeftAttribute.ConstantValue,
                            predicate.GetComType());
                    }
                    else if (predicate.LeftAttribute.Attribute != null && predicate.RightAttribute.ConstantValue != null)
                    {
                        result = FilterEstimator.GetEstimationResult(
                                result,
                                predicate.LeftAttribute.Attribute,
                                predicate.RightAttribute.ConstantValue,
                                predicate.GetComType());
                    }
                    else
                        throw new Exception("Impossible predicate detected.");
                }
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
