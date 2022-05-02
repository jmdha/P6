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
using QueryEstimator.Helpers;

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
                var segmentResultChain = GetIntermediateResults(query);
                var totalEstimation = segmentResultChain.GetTotalEstimation();

                return new EstimatorResult(query, (ulong)totalEstimation);
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
            foreach (var filter in _scanner.GetIfThere<FilterPredicate>())
                filterBounds.Add(SimpleFilterBounder.Bound(
                    filter.LeftTable,
                    filter.ConstantValue,
                    filter.ComType));

            // Get estiamtes from predicates
            foreach (var predicate in _scanner.GetIfThere<TableAttributePredicate>())
            {
                // Get bounds for left and right tables, both ways
                tableAttributeBounds.Add(TableAttributeBounder.Bound(
                    predicate.LeftTable,
                    predicate.RightTable,
                    predicate.ComType));
                tableAttributeBounds.Add(TableAttributeBounder.Bound(
                    predicate.RightTable,
                    predicate.LeftTable,
                    ComparisonTypeHelper.InvertType(predicate.ComType)));
            }

            // Sweep all bounds to see of some need to be changed
            bool anyChanged = false;
            for (int i = 0; i < _maxSweeps; i++)
            {
                if (CheckAndRecalculateFilterBounds(filterBounds))
                    anyChanged = true;
                if (CheckAndRecalculateTableAttributeBounds(tableAttributeBounds))
                    anyChanged = true;
                if (!anyChanged)
                    break;
            }
        }

        private bool CheckAndRecalculateFilterBounds(List<IPredicateBoundResult<IComparable>> bounds)
        {
            bool anyChanged = false;
            for (int i = 0; i < bounds.Count; i++)
            {
                if (bounds[i].LowerBound != _lowerBounds[bounds[i].Left] || bounds[i].UpperBound != _upperBounds[bounds[i].Left])
                {
                    anyChanged = true;
                    bounds[i].RecalculateBounds();
                }
            }
            return anyChanged;
        }

        private bool CheckAndRecalculateTableAttributeBounds(List<IPredicateBoundResult<TableAttribute>> bounds)
        {
            bool anyChanged = false;
            for (int i = 0; i < bounds.Count; i += 2)
            {
                if (bounds[i].LowerBound > _lowerBounds[bounds[i].Left] || bounds[i].UpperBound > _upperBounds[bounds[i].Left] ||
                        bounds[i + 1].LowerBound > _lowerBounds[bounds[i + 1].Left] || bounds[i + 1].UpperBound > _upperBounds[bounds[i + 1].Left])
                {
                    anyChanged = true;
                    bounds[i].RecalculateBounds();
                    bounds[i + 1].RecalculateBounds();
                }
            }
            return anyChanged;
        }



        private ISegmentResult GetEstimationForSegments()
        {
            ISegmentResult result = new ValueTableAttributeResult(_initAttribute, _initAttribute, 1, ComparisonType.Type.None);

            // Foreach TableAttribute predicate in the join query, get an estimation based on new bounds.
            foreach (var pred in _scanner.GetIfThere<TableAttributePredicate>())
            {
                result = new SegmentResult(result, TableAttributeEstimator.GetEstimationResult(
                    result,
                    pred.LeftTable,
                    pred.RightTable,
                    pred.ComType));
            }
            return result;
        }
    }
}
