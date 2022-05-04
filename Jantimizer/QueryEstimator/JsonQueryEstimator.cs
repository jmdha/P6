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
using Segmentator;

namespace QueryEstimator
{
    public class JsonQueryEstimator : IQueryEstimator<JsonQuery>
    {
        public IMilestoner Milestoner { get; }

        // Scanners
        public IPredicateScanner<List<JoinNode>> PredicateScanner { get; }
        // Bounders
        public IPredicateBounder<IComparable> SimpleFilterBounder { get; }
        public IPredicateBounder<TableAttribute> TableAttributeBounder { get; }
        // Estimators
        public IPredicateEstimator<TableAttribute> TableAttributeEstimator { get; }

        private Dictionary<TableAttribute, int> _upperBounds;
        private Dictionary<TableAttribute, int> _lowerBounds;
        private int _maxSweeps;
        private int _didSweeps = 0;

        public JsonQueryEstimator(IMilestoner milestoner, int maxSweeps)
        {
            Milestoner = milestoner;
            _upperBounds = new Dictionary<TableAttribute, int>();
            _lowerBounds = new Dictionary<TableAttribute, int>();
            TableAttributeEstimator = new TableAttributeEstimator(_upperBounds, _lowerBounds, Milestoner);
            SimpleFilterBounder = new SimpleFilterBounder(_upperBounds, _lowerBounds, Milestoner);
            TableAttributeBounder = new TableAttributeBounder(_upperBounds, _lowerBounds, Milestoner);
            _maxSweeps = maxSweeps;
            PredicateScanner = new JoinPredicateScanner();
        }

        public EstimatorResult GetQueryEstimation(JsonQuery query)
        {
            try
            {
                _didSweeps = 0;
                var totalEstimation = GetTotalEstimationForQuery(query);

                return new EstimatorResult(query, (ulong)totalEstimation, _didSweeps);
            }
            catch (Exception ex)
            {
                throw new EstimatorErrorLogException(ex, query);
            }
        }

        private long GetTotalEstimationForQuery(JsonQuery query)
        {
            // Clear previous bounds
            _upperBounds.Clear();
            _lowerBounds.Clear();

            // Phase One
            // Scan the query for predicates
            PredicateScanner.Scan(query.JoinNodes);

            // Phase Two
            // Set bounds
            BoundTableAttributes();

            // Phase Three
            // Get estiamte chain
            var segmentChain = GetEstimationForSegments();

            // Phase Four
            // Get total estimate for all predicates
            var result = segmentChain.GetTotalEstimation();

            return result;
        }

        private void BoundTableAttributes()
        {
            // Get bounds for filters
            var filterBounds = GetBoundsForFilters();
            // Get bounds for table attribute predicates
            var tableAttributeBounds = GetBoundsForTableAttributePredicates();

            // Sweep all bounds to see of some need to be changed
            SweepBounds(filterBounds, tableAttributeBounds);
        }

        private List<IPredicateBoundResult<TableAttribute>> GetBoundsForTableAttributePredicates()
        {
            var tableAttributeBounds = new List<IPredicateBoundResult<TableAttribute>>();
            foreach (var predicate in PredicateScanner.GetIfThere<TableAttributePredicate>())
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
            return tableAttributeBounds;
        }

        private List<IPredicateBoundResult<IComparable>> GetBoundsForFilters()
        {
            var filterBounds = new List<IPredicateBoundResult<IComparable>>();
            foreach (var filter in PredicateScanner.GetIfThere<FilterPredicate>())
                filterBounds.Add(SimpleFilterBounder.Bound(
                    filter.LeftTable,
                    filter.ConstantValue,
                    filter.ComType));
            return filterBounds;
        }

        private void SweepBounds(List<IPredicateBoundResult<IComparable>> filterBounds, List<IPredicateBoundResult<TableAttribute>> tableAttributeBounds)
        {
            bool anyChanged = false;
            for (int i = 0; i < _maxSweeps; i++)
            {
                if (CheckAndRecalculateFilterBounds(filterBounds))
                    anyChanged = true;
                if (CheckAndRecalculateTableAttributeBounds(tableAttributeBounds))
                    anyChanged = true;
                if (!anyChanged)
                    break;
                _didSweeps++;
            }
        }

        private bool CheckAndRecalculateFilterBounds(List<IPredicateBoundResult<IComparable>> bounds)
        {
            bool anyChanged = false;
            for (int i = 0; i < bounds.Count; i++)
            {
                if (bounds[i].HaveChanged(_lowerBounds, _upperBounds))
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
                if (bounds[i].HaveChanged(_lowerBounds, _upperBounds) || bounds[i + 1].HaveChanged(_lowerBounds, _upperBounds))
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
            ISegmentResult result = new ValueTableAttributeResult(new TableAttribute(), new TableAttribute(), 1, ComparisonType.Type.None);

            // Foreach TableAttribute predicate in the join query, get an estimation based on new bounds.
            foreach (var pred in PredicateScanner.GetIfThere<TableAttributePredicate>())
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
