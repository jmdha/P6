using Histograms;
using Histograms.Models;
using QueryEstimator.Models;
using QueryEstimator.Models.BoundResults;
using QueryEstimator.SegmentHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.Models.JsonModels;

namespace QueryEstimator.PredicateBounders
{
    public class TableAttributeBounder : BaseSegmentBoundsHandler, IPredicateBounder<TableAttribute>
    {
        private IHistogramSegmentationComparative? _lastEqual = null;
        public TableAttributeBounder(Dictionary<TableAttribute, int> upperBounds, Dictionary<TableAttribute, int> lowerBounds, IHistogramManager histogramManager) : base(upperBounds, lowerBounds, histogramManager)
        {
        }

        public IPredicateBoundResult<TableAttribute> Bound(TableAttribute source, TableAttribute compare, ComparisonType.Type type)
        {
            // Get segments and lower/upper bounds for the source attribute
            var allSourceSegments = GetAllSegmentsForAttribute(source);
            int currentSourceLowerBound = GetLowerBoundOrAlt(source, 0);
            int newSourceLowerBound = currentSourceLowerBound;
            int currentSourceUpperBound = GetUpperBoundOrAlt(source, allSourceSegments.Count - 1);
            int newSourceUpperBound = currentSourceUpperBound;

            // Initialise the new source bounds for edge cases
            if (type == ComparisonType.Type.More || type == ComparisonType.Type.EqualOrMore)
            {
                newSourceLowerBound = currentSourceUpperBound;
                newSourceUpperBound = currentSourceUpperBound;
            }
            else if (type == ComparisonType.Type.Less || type == ComparisonType.Type.EqualOrLess)
            {
                newSourceLowerBound = 0;
                newSourceUpperBound = -1;
            }

            // Only check for new bounds if the bound have not already been reduced to the max
            if (currentSourceLowerBound < currentSourceUpperBound)
            {
                // Skip the first lower bound if the predicate is equal
                if (type == ComparisonType.Type.Equal)
                {
                    _lastEqual = allSourceSegments[currentSourceLowerBound];
                    currentSourceLowerBound++;
                }

                // Check within the bounds until a given predicate is no longer correct
                bool foundAny = false;
                for (int i = currentSourceLowerBound; i <= currentSourceUpperBound; i++)
                {
                    bool isAny = false;
                    switch (type)
                    {
                        case ComparisonType.Type.Equal:
                            if (_lastEqual == null)
                                throw new ArgumentNullException("_lastEqual cannot be null");
                            isAny = allSourceSegments[i].IsAnySmallerThanNoAlias(compare) && _lastEqual.IsAnyLargerThanNoAlias(compare);
                            _lastEqual = allSourceSegments[i];
                            break;
                        case ComparisonType.Type.More:
                        case ComparisonType.Type.EqualOrMore:
                            isAny = allSourceSegments[i].IsAnySmallerThanNoAlias(compare);
                            break;
                        case ComparisonType.Type.Less:
                        case ComparisonType.Type.EqualOrLess:
                            isAny = allSourceSegments[i].IsAnyLargerThanNoAlias(compare);
                            break;
                    }

                    if (!isAny && foundAny)
                    {
                        newSourceUpperBound = i - 1;
                        break;
                    }
                    else if (!isAny)
                        newSourceLowerBound = i;
                    else
                        foundAny = true;
                }

                // Add to dictionary if not there
                AddOrReduceUpperBound(source, newSourceUpperBound);
                AddOrReduceLowerBound(source, newSourceLowerBound);
            }

            // Return a new bound result with the new upper and lower bounds
            return new PredicateBoundResult<TableAttribute>(this, source, compare, type, newSourceUpperBound, newSourceLowerBound);
        }
    }
}
