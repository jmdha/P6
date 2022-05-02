using Histograms;
using Histograms.Models;
using QueryEstimator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.Models.JsonModels;

namespace QueryEstimator.PredicateEstimators
{
    public class TableAttributeEstimator : BasePredicateEstimator<TableAttribute>
    {
        public TableAttributeEstimator(Dictionary<TableAttribute, int> upperBounds, Dictionary<TableAttribute, int> lowerBounds, IHistogramManager histogramManager) : base(upperBounds, lowerBounds, histogramManager)
        {
        }

        public override ISegmentResult GetEstimationResult(ISegmentResult current, TableAttribute source, TableAttribute compare, ComparisonType.Type type)
        {
            long newResult = 0;
            bool doesPreviousContain = false;
            if (current.DoesContainTableAttribute(source) || current.DoesContainTableAttribute(compare))
                doesPreviousContain = true;

            // Get segments and lower/upper bounds for the source attribute
            var allSourceSegments = GetAllSegmentsForAttribute(source);
            int sourceLowerBound = GetLowerBoundOrAlt(source, 0);
            int sourceUpperBound = GetUpperBoundOrAlt(source, allSourceSegments.Count - 1);

            // Get segments and lower/upper bounds for the compare attribute
            // Also get how many items that should be bounded in the compare segments.
            var allcompareSegments = GetAllSegmentsForAttribute(compare);
            int compareLowerBound = GetLowerBoundOrAlt(compare, 0);
            int compareUpperBound = GetUpperBoundOrAlt(compare, allcompareSegments.Count - 1);
            long bottomBoundsSmallerCount = (long)allcompareSegments[compareLowerBound].GetCountSmallerThanNoAlias(compare);
            long bottomBoundsLargerCount = (long)allcompareSegments[compareLowerBound].GetCountLargerThanNoAlias(compare);
            long topBoundsSmallCount = (long)allcompareSegments[compareUpperBound].GetCountSmallerThanNoAlias(compare);
            long topBoundsLargerCount = (long)allcompareSegments[compareUpperBound].GetCountLargerThanNoAlias(compare);

            IHistogramSegmentationComparative lastEqual = allSourceSegments[sourceLowerBound];
            if (type == ComparisonType.Type.Equal)
                sourceLowerBound++;

            for (int i = sourceLowerBound; i <= sourceUpperBound; i++)
            {
                switch (type)
                {
                    case ComparisonType.Type.More:
                    case ComparisonType.Type.EqualOrMore:
                        newResult += GetBoundedSegmentResult(
                            allSourceSegments[i],
                            (long)allSourceSegments[i].GetCountSmallerThanNoAlias(compare),
                            doesPreviousContain, bottomBoundsSmallerCount, topBoundsSmallCount);
                        break;
                    case ComparisonType.Type.Less:
                    case ComparisonType.Type.EqualOrLess:
                        newResult += GetBoundedSegmentResult(
                            allSourceSegments[i],
                            (long)allSourceSegments[i].GetCountLargerThanNoAlias(compare),
                            doesPreviousContain, topBoundsLargerCount, bottomBoundsLargerCount);
                        break;
                    case ComparisonType.Type.Equal:
                        long belowThis = GetBoundedSegmentResult(allSourceSegments[i], (long)allSourceSegments[i].GetCountSmallerThanNoAlias(compare), true, bottomBoundsSmallerCount, topBoundsSmallCount);
                        long belowPreviousThis = GetBoundedSegmentResult(lastEqual, (long)lastEqual.GetCountSmallerThanNoAlias(compare), true, bottomBoundsSmallerCount, topBoundsSmallCount); ;
                        newResult += (belowThis - belowPreviousThis);
                        lastEqual = allSourceSegments[i];
                        break;
                }
            }

            return new ValueTableAttributeResult(UpperBounds[source], LowerBounds[source], source, UpperBounds[compare], LowerBounds[compare], compare, newResult, type);
        }

        private long GetBoundedSegmentResult(IHistogramSegmentationComparative segment, long add, bool doesPreviousContain, long bottomOffsetCount, long checkOffsetCount)
        {
            if (add > checkOffsetCount)
                add -= (add - checkOffsetCount);
            if (bottomOffsetCount > 0)
                add -= bottomOffsetCount;
            if (add < 0)
                return 0;
            if (doesPreviousContain)
                return add;
            else
                return add * segment.ElementsBeforeNextSegmentation;
        }
    }
}
