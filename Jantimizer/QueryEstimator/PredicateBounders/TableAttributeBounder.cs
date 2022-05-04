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


            // Only check for new bounds if the bound have not already been reduced to the max
            if (currentSourceLowerBound <= currentSourceUpperBound)
            {
                // Check within the bounds until a given predicate is no longer correct
                bool foundAny = false;
                for (int i = currentSourceLowerBound; i <= currentSourceUpperBound; i++)
                {
                    bool isAny = false;
                    switch (type)
                    {
                        case ComparisonType.Type.Equal:
                            isAny = allSourceSegments[i].IsAnySmallerThanNoAlias(compare) && allSourceSegments[i].IsAnyLargerThanNoAlias(compare);
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
                    else if (isAny && !foundAny && i == currentSourceUpperBound)
                    {
                        newSourceLowerBound = i;
                        newSourceUpperBound = i;
                        foundAny = true;
                        break;
                    }
                    else if (!isAny)
                        newSourceLowerBound = i;
                    else
                        foundAny = true;
                }

                if (!foundAny)
                {
                    newSourceLowerBound = currentSourceLowerBound;
                    newSourceUpperBound = -1;
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
