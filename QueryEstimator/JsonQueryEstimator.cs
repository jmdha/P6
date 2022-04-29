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
using QueryEstimator.Models.PredicateScanners;
using QueryEstimator.PredicateScanners;

namespace QueryEstimator
{
    public class JsonQueryEstimator : IQueryEstimator<JsonQuery>
    {
        public CrossFilterEstimator CrossFilterEstimator { get; }
        public SimpleFilterEstimator SimpleFilterEstimator { get; }
        public TableAttributeEstimator TableAttributeEstimator { get; }
        public IHistogramManager HistogramManager { get; }

        private Dictionary<TableAttribute, int> _upperBounds;
        private Dictionary<TableAttribute, int> _lowerBounds;
        private Dictionary<string, int> _upperRowBounds;
        private Dictionary<string, int> _lowerRowBounds;
        private TableAttribute _initAttribute = new TableAttribute();
        private int _maxSweeps = 0;
        private JsonQuery _currentQuery = new JsonQuery();
        private IPredicateScanner<List<JoinNode>> _scanner;

        public JsonQueryEstimator(IHistogramManager histogramManager, int maxSweeps)
        {
            HistogramManager = histogramManager;
            _upperBounds = new Dictionary<TableAttribute, int>();
            _lowerBounds = new Dictionary<TableAttribute, int>();
            _upperRowBounds = new Dictionary<string, int>();
            _lowerRowBounds = new Dictionary<string, int>();
            CrossFilterEstimator = new CrossFilterEstimator(_upperRowBounds, _lowerRowBounds, histogramManager);
            TableAttributeEstimator = new TableAttributeEstimator(_upperBounds, _lowerBounds, histogramManager);
            SimpleFilterEstimator = new SimpleFilterEstimator(_upperBounds, _lowerBounds, histogramManager);
            _maxSweeps = maxSweeps;
            _scanner = new JoinPredicateScanner();
        }

        public EstimatorResult GetQueryEstimation(JsonQuery query)
        {
            try
            {
                _currentQuery = query;
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

            ISegmentResult result = new ValueTableAttributeResult(0,0, _initAttribute, 0,0, _initAttribute, 1, ComparisonType.Type.None);

            _scanner.Scan(query.JoinNodes);

            // Limit bounds for simple filters
            foreach (var pred in _scanner.GetIfThere(typeof(SimpleFilterPredicate)))
            {
                if (pred is SimpleFilterPredicate filter)
                    SimpleFilterEstimator.GetEstimationResult(
                        result,
                        filter.LeftTable,
                        filter.ConstantValue,
                        filter.ComType);
            }

            // Limit bounds for cross filters
            foreach (var pred in _scanner.GetIfThere(typeof(CrossFilterPredicate)))
            {
                if (pred is CrossFilterPredicate filter)
                    CrossFilterEstimator.GetEstimationResult(
                        result,
                        filter.LeftTable,
                        filter.ConstantValue,
                        filter.ComType);
            }

            // Get estiamtes from predicates
            foreach (var pred in _scanner.GetIfThere(typeof(TableAttributePredicate)))
            {
                if (pred is TableAttributePredicate predicate)
                    result = TableAttributeEstimator.GetEstimationResult(
                        result,
                        predicate.LeftTable,
                        predicate.RightTable,
                        predicate.ComType);
            }

            // Sweep all results for bounds changes
            result = SweepAllSegments(result);

            return result;
        }

        private ISegmentResult SweepAllSegments(ISegmentResult result)
        {
            int currentHash = result.GetHashCode();
            for (int i = 0; i < _maxSweeps; i++)
            {
                result = SweepSegments(result);
                var newHash = result.GetHashCode();
                if (newHash == currentHash)
                    break;
                currentHash = newHash;
            }
            return result;
        }

        private ISegmentResult SweepSegments(ISegmentResult current)
        {
            if (current is SegmentResult seg)
            {
                seg.Left = SweepSegments(seg.Left);
                seg.Right = SweepSegments(seg.Right);
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
                    return CrossFilterEstimator.GetEstimationResult(
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
