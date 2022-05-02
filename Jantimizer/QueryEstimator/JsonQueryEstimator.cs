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
using QueryEstimator.PredicateBounders;
using QueryEstimator.Models.BoundResults;

namespace QueryEstimator
{
    public class JsonQueryEstimator : IQueryEstimator<JsonQuery>
    {
        public SimpleFilterBounder SimpleFilterBounder { get; }
        public TableAttributeBounder TableAttributeBounder { get; }
        public TableAttributeEstimator TableAttributeEstimator { get; }
        public IHistogramManager HistogramManager { get; }

        private Dictionary<TableAttribute, int> _upperBounds;
        private Dictionary<TableAttribute, int> _lowerBounds;
        private TableAttribute _initAttribute = new TableAttribute();
        private int _maxSweeps;
        private JsonQuery _currentQuery = new JsonQuery();
        private IPredicateScanner<List<JoinNode>> _scanner;

        public JsonQueryEstimator(IHistogramManager histogramManager, int maxSweeps)
        {
            HistogramManager = histogramManager;
            _upperBounds = new Dictionary<TableAttribute, int>();
            _lowerBounds = new Dictionary<TableAttribute, int>();
            TableAttributeEstimator = new TableAttributeEstimator(_upperBounds, _lowerBounds, histogramManager);
            SimpleFilterBounder = new SimpleFilterBounder(_upperBounds, _lowerBounds, histogramManager);
            TableAttributeBounder = new TableAttributeBounder(_upperBounds, _lowerBounds, histogramManager);
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
            // Clear previous bounds
            _upperBounds.Clear();
            _lowerBounds.Clear();

            // Scan the query for predicates
            _scanner.Scan(query.JoinNodes);

            // Set bounds
            BoundAttributes();

            // Get estiamtes from predicates
            var result = GetEstimationForSegments();

            return result;
        }

        private void BoundAttributes()
        {
            var filterBounds = new List<IPredicateBoundResult<IComparable>>();
            var tableAttributeBounds = new List<IPredicateBoundResult<TableAttribute>>();

            // Limit bounds for simple filters
            foreach (var pred in _scanner.GetIfThere(typeof(SimpleFilterPredicate)))
            {
                if (pred is SimpleFilterPredicate filter)
                    filterBounds.Add(SimpleFilterBounder.Bound(
                        filter.LeftTable,
                        filter.ConstantValue,
                        filter.ComType));
            }

            // Get estiamtes from predicates
            foreach (var pred in _scanner.GetIfThere(typeof(TableAttributePredicate)))
            {
                if (pred is TableAttributePredicate predicate)
                {
                    // Get bounds for left and right tables, both ways
                    tableAttributeBounds.Add(TableAttributeBounder.Bound(
                        predicate.LeftTable,
                        predicate.RightTable,
                        predicate.ComType));
                    tableAttributeBounds.Add(TableAttributeBounder.Bound(
                        predicate.RightTable,
                        predicate.LeftTable,
                        InvertType(predicate.ComType)));
                }
            }

            // Sweep all bounds to see of some need to be changed
            bool anyChanged = false;
            for (int i = 0; i < _maxSweeps; i++)
            {
                if (CheckBounds(filterBounds))
                    anyChanged = true;
                if (CheckBounds(tableAttributeBounds))
                    anyChanged = true;
                if (!anyChanged)
                    break;
            }
        }

        private bool CheckBounds<TRight>(List<IPredicateBoundResult<TRight>> bounds)
        {
            bool anyChanged = false;
            for (int i = 0; i < bounds.Count; i++)
            {
                if (typeof(TRight) == typeof(TableAttribute))
                {
                    if (bounds[i].LowerBound > _lowerBounds[bounds[i].Left] || bounds[i].UpperBound > _upperBounds[bounds[i].Left] ||
                        bounds[i + 1].LowerBound > _lowerBounds[bounds[i + 1].Left] || bounds[i + 1].UpperBound > _upperBounds[bounds[i + 1].Left])
                    {
                        anyChanged = true;
                        bounds[i].RecalculateBounds();
                        bounds[i + 1].RecalculateBounds();
                    }
                    i++;
                }
                else if (typeof(TRight) == typeof(IComparable))
                {
                    if (bounds[i].LowerBound != _lowerBounds[bounds[i].Left] || bounds[i].UpperBound != _upperBounds[bounds[i].Left])
                    {
                        anyChanged = true;
                        bounds[i].RecalculateBounds();
                    }
                }
            }
            return anyChanged;
        }

        private ComparisonType.Type InvertType(ComparisonType.Type fromType)
        {
            switch (fromType)
            {
                case ComparisonType.Type.Less: return ComparisonType.Type.EqualOrMore;
                case ComparisonType.Type.More: return ComparisonType.Type.EqualOrLess;
                case ComparisonType.Type.EqualOrLess: return ComparisonType.Type.More;
                case ComparisonType.Type.EqualOrMore: return ComparisonType.Type.Less;
                case ComparisonType.Type.Equal: return ComparisonType.Type.Equal;
                default:
                    throw new Exception("Impossible comparison type");
            }
        }

        private ISegmentResult GetEstimationForSegments()
        {
            ISegmentResult result = new ValueTableAttributeResult(0, 0, _initAttribute, 0, 0, _initAttribute, 1, ComparisonType.Type.None);
            foreach (var pred in _scanner.GetIfThere(typeof(TableAttributePredicate)))
            {
                if (pred is TableAttributePredicate predicate)
                    result = new SegmentResult(result, TableAttributeEstimator.GetEstimationResult(
                        result,
                        predicate.LeftTable,
                        predicate.RightTable,
                        predicate.ComType));
            }
            return result;
        }
    }
}
