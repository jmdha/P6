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
using Milestoner.Models.Milestones;

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
                // Get segments and lower/upper bounds for the compare attribute
                // Also get how many items that should be bounded in the compare segments.
                var allcompareSegments = GetAllMilestonesForAttribute(compare);
                int compareLowerBound = GetLowerBoundOrAlt(compare, 0);
                int compareUpperBound = GetUpperBoundOrAlt(compare, allcompareSegments.Count - 1);
                if (compareLowerBound <= compareUpperBound)
                {
                    long bottomBoundsSmallerCount = GetCountSmallerThan(allcompareSegments[compareLowerBound], compare);
                    long bottomBoundsLargerCount = GetCountLargerThan(allcompareSegments[compareLowerBound], compare) + allcompareSegments[compareLowerBound].ElementsBeforeNextSegmentation;
                    long topBoundsSmallCount = GetCountSmallerThan(allcompareSegments[compareUpperBound], compare) + allcompareSegments[compareUpperBound].ElementsBeforeNextSegmentation;
                    long topBoundsLargerCount = GetCountLargerThan(allcompareSegments[compareUpperBound], compare);
                    
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
                                isAny = IsAny_Equal(allSourceSegments[i], compare);
                                break;
                            case ComparisonType.Type.More:
                            case ComparisonType.Type.EqualOrMore:
                                isAny = IsAny_More(
                                            allSourceSegments[i],
                                            compare,
                                            bottomBoundsSmallerCount,
                                            topBoundsSmallCount);
                                break;
                            case ComparisonType.Type.Less:
                            case ComparisonType.Type.EqualOrLess:
                                isAny = IsAny_Less(
                                            allSourceSegments[i],
                                            compare,
                                            topBoundsLargerCount,
                                            bottomBoundsLargerCount);
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
                }

                // Add to dictionary if not there
                AddOrReduceUpperBound(source, newSourceUpperBound);
                AddOrReduceLowerBound(source, newSourceLowerBound);
            }

            // Return a new bound result with the new upper and lower bounds
            return new PredicateBoundResult<TableAttribute>(this, source, compare, type, allSourceSegments.Count - 1, newSourceUpperBound, 0, newSourceLowerBound);
        }

        private bool IsAny_Equal(IMilestone segment, TableAttribute compare)
        {
            return GetCountLargerThan(segment, compare) != 0 && GetCountSmallerThan(segment, compare) != 0;
        }

        private bool IsAny_More(IMilestone segment, TableAttribute compare, long bottomBoundsSmallerCount, long topBoundsSmallCount)
        {
            return GetBoundedSegmentResult(
                            segment.ElementsBeforeNextSegmentation,
                            GetCountSmallerThan(segment, compare),
                            bottomBoundsSmallerCount, topBoundsSmallCount) != 0;
        }

        private bool IsAny_Less(IMilestone segment, TableAttribute compare, long topBoundsLargerCount, long bottomBoundsLargerCount)
        {
            return GetBoundedSegmentResult(
                            segment.ElementsBeforeNextSegmentation,
                            GetCountLargerThan(segment, compare),
                            topBoundsLargerCount, bottomBoundsLargerCount) != 0;
        }

        internal long GetBoundedSegmentResult(long segmentCount, long addValue, long bottomOffsetCount, long checkOffsetCount)
        {
            if (addValue >= checkOffsetCount)
                addValue -= (addValue - checkOffsetCount);
            if (bottomOffsetCount > 0)
                addValue -= bottomOffsetCount;
            if (addValue <= 0)
                return 0;
            return addValue * segmentCount;
        }

        private long GetCountSmallerThan(IMilestone segment, TableAttribute compare)
        {
            return (long)segment.GetCountSmallerThanNoAlias(compare);
        }

        private long GetCountLargerThan(IMilestone segment, TableAttribute compare)
        {
            return (long)segment.GetCountLargerThanNoAlias(compare);
        }
    }
}
