using QueryEstimator.Models;
using QueryEstimator.Models.BoundResults;
using QueryEstimator.SegmentHandler;
using Milestoner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.Models.JsonModels;
using Tools.Helpers;

namespace QueryEstimator.PredicateBounders
{
    public class TableAttributeBounder : BaseSegmentBoundsHandler, IPredicateBounder<TableAttribute>
    {
        public TableAttributeBounder(Dictionary<TableAttribute, int> upperBounds, Dictionary<TableAttribute, int> lowerBounds, IMilestoner milestoner) : base(upperBounds, lowerBounds, milestoner)
        {
        }

        public IReboundableResult<TableAttribute> Bound(TableAttribute source, TableAttribute compare, ComparisonType.Type type)
        {
            // Get segments and lower/upper bounds for the source attribute
            var allSourceSegments = GetAllMilestonesForAttribute(source);
            int currentSourceLowerBound = GetLowerBoundOrAlt(source, 0);
            int newSourceLowerBound = currentSourceLowerBound;
            int currentSourceUpperBound = GetUpperBoundOrAlt(source, allSourceSegments.Count - 1);
            int newSourceUpperBound = currentSourceUpperBound;

            // Only check for new bounds if the bound have not already been reduced to the max
            if (currentSourceLowerBound <= currentSourceUpperBound)
            {
                // Check within the bounds until a given predicate is no longer correct
                bool foundAny = false;
                for (int i = currentSourceLowerBound; i <= currentSourceUpperBound; i++)
                {
                    if (!foundAny)
                        newSourceLowerBound = i;
                    bool isAny = false;
                    switch (type)
                    {
                        case ComparisonType.Type.Equal:
                            isAny = allSourceSegments[i].IsAnyLargerThanNoAlias(compare) && allSourceSegments[i].IsAnySmallerThanNoAlias(compare);
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
                    else if (isAny && foundAny && i == currentSourceUpperBound)
                    {
                        newSourceUpperBound = i;
                        break;
                    }
                    else
                        if (isAny)
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
            return new PredicateBoundResult<TableAttribute>(this, source, compare, type, allSourceSegments.Count - 1, newSourceUpperBound, 0, newSourceLowerBound);
        }
    }
}
