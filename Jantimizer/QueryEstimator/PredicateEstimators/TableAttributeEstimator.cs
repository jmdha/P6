using Histograms;
using Histograms.Models;
using QueryEstimator.Models;
using QueryEstimator.SegmentHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Tools.Models.JsonModels;

[assembly: InternalsVisibleTo("QueryEstimatorTests")]

namespace QueryEstimator.PredicateEstimators
{
    public class TableAttributeEstimator : BaseSegmentBoundsHandler, IPredicateEstimator<TableAttribute>
    {
        public TableAttributeEstimator(Dictionary<TableAttribute, int> upperBounds, Dictionary<TableAttribute, int> lowerBounds, IHistogramManager histogramManager) : base(upperBounds, lowerBounds, histogramManager)
        {
        }

        public ISegmentResult GetEstimationResult(ISegmentResult current, TableAttribute source, TableAttribute compare, ComparisonType.Type type)
        {
            long newResult = 0;

            // Get segments and lower/upper bounds for the source attribute
            var allSourceSegments = GetAllSegmentsForAttribute(source);
            int sourceLowerBound = GetLowerBoundOrAlt(source, 0);
            int sourceUpperBound = GetUpperBoundOrAlt(source, allSourceSegments.Count - 1);

            if (sourceLowerBound <= sourceUpperBound)
            {
                // Check if the current segment results have already joined on either the source or compare table attribute
                bool doesPreviousContain = false;
                if (current.DoesContainTableAttribute(source) || current.DoesContainTableAttribute(compare))
                {
                    for (int i = sourceLowerBound; i <= sourceUpperBound; i++)
                        newResult += allSourceSegments[i].ElementsBeforeNextSegmentation;
                }
                else
                {
                    // Get segments and lower/upper bounds for the compare attribute
                    // Also get how many items that should be bounded in the compare segments.
                    var allcompareSegments = GetAllSegmentsForAttribute(compare);
                    int compareLowerBound = GetLowerBoundOrAlt(compare, 0);
                    int compareUpperBound = GetUpperBoundOrAlt(compare, allcompareSegments.Count - 1);
                    if (compareLowerBound <= compareUpperBound)
                    {
                        long bottomBoundsSmallerCount = GetCountSmallerThan(allcompareSegments[compareLowerBound], compare);
                        long bottomBoundsLargerCount = GetCountLargerThan(allcompareSegments[compareLowerBound], compare) + allcompareSegments[compareLowerBound].ElementsBeforeNextSegmentation;
                        long topBoundsSmallCount = GetCountSmallerThan(allcompareSegments[compareUpperBound], compare) + allcompareSegments[compareUpperBound].ElementsBeforeNextSegmentation;
                        long topBoundsLargerCount = GetCountLargerThan(allcompareSegments[compareUpperBound], compare);

                        for (int i = sourceLowerBound; i <= sourceUpperBound; i++)
                        {
                            switch (type)
                            {
                                case ComparisonType.Type.More:
                                case ComparisonType.Type.EqualOrMore:
                                    newResult += GetEstimatedValues_More(
                                        allSourceSegments[i],
                                        compare,
                                        doesPreviousContain,
                                        bottomBoundsSmallerCount,
                                        topBoundsSmallCount);
                                    break;
                                case ComparisonType.Type.Less:
                                case ComparisonType.Type.EqualOrLess:
                                    newResult += GetEstimatedValues_Less(
                                        allSourceSegments[i],
                                        compare,
                                        doesPreviousContain,
                                        topBoundsLargerCount,
                                        bottomBoundsLargerCount);
                                    break;
                                case ComparisonType.Type.Equal:
                                    newResult += GetEstimatedValues_Equal(allSourceSegments[i]);
                                    break;
                            }
                        }
                    }
                }
            }

            return new ValueTableAttributeResult(source, compare, newResult, type);
        }

        private long GetEstimatedValues_More(IHistogramSegmentationComparative segment, TableAttribute compare, bool doesPreviousContain, long bottomBoundsSmallerCount, long topBoundsSmallCount)
        {
            return GetBoundedSegmentResult(
                            segment.ElementsBeforeNextSegmentation,
                            (long)segment.GetCountSmallerThanNoAlias(compare),
                            doesPreviousContain, bottomBoundsSmallerCount, topBoundsSmallCount);
        }

        private long GetEstimatedValues_Less(IHistogramSegmentationComparative segment, TableAttribute compare, bool doesPreviousContain, long topBoundsLargerCount, long bottomBoundsLargerCount)
        {
            return GetBoundedSegmentResult(
                            segment.ElementsBeforeNextSegmentation,
                            (long)segment.GetCountLargerThanNoAlias(compare),
                            doesPreviousContain, topBoundsLargerCount, bottomBoundsLargerCount);
        }

        private long GetEstimatedValues_Equal(IHistogramSegmentationComparative segment)
        {
            return segment.ElementsBeforeNextSegmentation;
        }

        internal long GetBoundedSegmentResult(long segmentCount, long addValue, bool doesPreviousContain, long bottomOffsetCount, long checkOffsetCount)
        {
            if (addValue >= checkOffsetCount)
                addValue -= (addValue - checkOffsetCount);
            if (bottomOffsetCount > 0)
                addValue -= bottomOffsetCount;
            if (addValue <= 0)
                return 0;
            if (doesPreviousContain)
                return segmentCount;
            else
                return addValue * segmentCount;
        }

        private long GetCountSmallerThan(IHistogramSegmentationComparative segment, TableAttribute compare)
        {
            return (long)segment.GetCountSmallerThanNoAlias(compare);
        }

        private long GetCountLargerThan(IHistogramSegmentationComparative segment, TableAttribute compare)
        {
            return (long)segment.GetCountLargerThanNoAlias(compare);
        }
    }
}
