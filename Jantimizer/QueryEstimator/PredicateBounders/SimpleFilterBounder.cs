using QueryEstimator.Models;
using QueryEstimator.Models.BoundResults;
using QueryEstimator.SegmentHandler;
using Milestoner;
using Milestoner.Models.Milestones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Tools.Helpers;
using Tools.Models.JsonModels;

[assembly:InternalsVisibleTo("QueryEstimatorTests")]

namespace QueryEstimator.PredicateBounders
{
    public class SimpleFilterBounder : BaseSegmentBoundsHandler, IPredicateBounder<IComparable>
    {
        public SimpleFilterBounder(Dictionary<TableAttribute, int> upperBounds, Dictionary<TableAttribute, int> lowerBounds, IMilestoner milestoner) : base(upperBounds, lowerBounds, milestoner)
        {
        }

        public IReboundableResult<IComparable> Bound(TableAttribute source, IComparable compare, ComparisonType.Type type)
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
                // Convert the type to compare against to be the same as the one in the segments (assuming its correct)
                compare = ConvertCompareTypes(allSourceSegments[currentSourceLowerBound], compare);

                if (type == ComparisonType.Type.Equal)
                {
                    newSourceLowerBound = 0;
                    newSourceUpperBound = -1;
                }

                // Check within the bounds until a given predicate is no longer correct
                bool foundLower = false;
                bool exitSentinel = false;
                for (int i = currentSourceLowerBound; i <= currentSourceUpperBound; i++)
                {
                    switch (type)
                    {
                        case ComparisonType.Type.More:
                            newSourceLowerBound = i;
                            if (allSourceSegments[i].LowestValue.IsLargerThan(compare))
                                exitSentinel = true;
                            break;
                        case ComparisonType.Type.Less:
                            if (allSourceSegments[i].HighestValue.IsLargerThanOrEqual(compare))
                            {
                                if (newSourceUpperBound == currentSourceUpperBound)
                                    newSourceUpperBound = -1;
                                exitSentinel = true;
                                break;
                            }
                            newSourceUpperBound = i;
                            break;
                        case ComparisonType.Type.Equal:
                            if (!foundLower)
                            {
                                if (allSourceSegments[i].LowestValue.IsLargerThan(compare) && allSourceSegments[i].HighestValue.IsLargerThan(compare))
                                {
                                    newSourceLowerBound = 0;
                                    newSourceUpperBound = -1;
                                    exitSentinel = true;
                                    break;
                                }
                                else if (allSourceSegments[i].HighestValue.IsLargerThanOrEqual(compare))
                                {
                                    newSourceLowerBound = i;
                                    newSourceUpperBound = i;
                                    foundLower = true;
                                }
                            }
                            else
                            {
                                if (allSourceSegments[i].HighestValue.IsLessThanOrEqual(compare))
                                    newSourceUpperBound = i;
                                else
                                    exitSentinel = true;

                            }
                            break;
                    }
                    if (exitSentinel)
                        break;
                }

                // Add to dictionary if not there
                AddOrReduceUpperBound(source, newSourceUpperBound);
                AddOrReduceLowerBound(source, newSourceLowerBound);
            }

            // Return a new bound result with the new upper and lower bounds
            return new PredicateBoundResult<IComparable>(this, source, compare, type, allSourceSegments.Count - 1, newSourceUpperBound, 0, newSourceLowerBound);
        }

        internal IComparable ConvertCompareTypes(IMilestone segment, IComparable compare)
        {
            var compType = compare.GetType();
            var valueType = segment.LowestValue.GetType();
            if (compType != valueType)
                return (IComparable)Convert.ChangeType(compare, valueType);
            return compare;
        }
    }
}
