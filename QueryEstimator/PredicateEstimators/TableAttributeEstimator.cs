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

            var allSourceSegments = GetAllSegmentsForAttribute(source);
            var allcompareSegments = GetAllSegmentsForAttribute(compare);
            int sourceLowerBound = GetLowerBoundOrAlt(source, 0);
            int sourceUpperBound = GetUpperBoundOrAlt(source, allSourceSegments.Count - 1);
            int compareLowerBound = GetLowerBoundOrAlt(compare, 0);
            int compareUpperBound = GetUpperBoundOrAlt(compare, allcompareSegments.Count - 1);

            long bottomBoundsOffset = GetBottomBoundsOffset(allcompareSegments, compareLowerBound, compare);
            long bottomBoundsCount = GetBottomBoundsCount(allcompareSegments, compareLowerBound, compare);
            long topBoundsOffset = GetTopBoundsOffset(allcompareSegments, compareUpperBound, compare);
            long topBoundsCount = GetTopBoundsCount(allcompareSegments, compareUpperBound, compare);

            if (type == ComparisonType.Type.More || type == ComparisonType.Type.EqualOrMore)
            {
                for (int i = sourceUpperBound; i >= sourceLowerBound; i--)
                {
                    long add = (long)allSourceSegments[i].GetCountSmallerThanNoAlias(compare);
                    newResult += AddSegmentResult(allSourceSegments[i], add, doesPreviousContain, bottomBoundsOffset, topBoundsCount);
                }
            } else if (type == ComparisonType.Type.Less || type == ComparisonType.Type.EqualOrLess)
            {
                for (int i = sourceLowerBound; i <= sourceUpperBound; i++)
                {
                    long add = (long)allSourceSegments[i].GetCountLargerThanNoAlias(compare);
                    newResult += AddSegmentResult(allSourceSegments[i], add, doesPreviousContain, bottomBoundsOffset, bottomBoundsCount);
                }
            } else if (type == ComparisonType.Type.Equal)
            {
                IHistogramSegmentationComparative lastEqual = allSourceSegments[sourceLowerBound];
                for (int i = sourceLowerBound; i <= sourceUpperBound; i++)
                {
                    long aboveThis = (long)allSourceSegments[i].GetCountLargerThanNoAlias(compare) - topBoundsOffset;
                    long abovePreviousThis = (long)lastEqual.GetCountLargerThanNoAlias(compare) - topBoundsOffset;
                    if (doesPreviousContain)
                        newResult += abovePreviousThis - aboveThis;
                    else
                        newResult += (abovePreviousThis - aboveThis) * allSourceSegments[i].ElementsBeforeNextSegmentation;
                    lastEqual = allSourceSegments[i];
                }
            }

            return new ValueTableAttributeResult(UpperBounds[source], LowerBounds[source], source, UpperBounds[compare], LowerBounds[compare], compare, newResult, type);
        }

        private long GetBottomBoundsOffset(List<IHistogramSegmentationComparative> segments, int compareIndex, TableAttribute compare)
        {
            return (long)segments[compareIndex].GetCountSmallerThanNoAlias(compare);
        }

        private long GetBottomBoundsCount(List<IHistogramSegmentationComparative> segments, int compareIndex, TableAttribute compare)
        {
            return (long)segments[compareIndex].GetCountLargerThanNoAlias(compare);
        }

        private long GetTopBoundsCount(List<IHistogramSegmentationComparative> segments, int compareIndex, TableAttribute compare)
        {
            return (long)segments[compareIndex].GetCountSmallerThanNoAlias(compare);
        }

        private long GetTopBoundsOffset(List<IHistogramSegmentationComparative> segments, int compareIndex, TableAttribute compare)
        {
            return (long)segments[compareIndex].GetCountLargerThanNoAlias(compare);
        }

        private long AddSegmentResult(IHistogramSegmentationComparative segment, long add, bool doesPreviousContain, long bottomOffset, long checkCountOffset)
        {
            if (add > checkCountOffset)
                add -= (add - checkCountOffset);
            if (bottomOffset > 0)
                add -= bottomOffset;
            if (add < 0)
                return 0;
            if (doesPreviousContain)
                return add;
            else
                return add * segment.ElementsBeforeNextSegmentation;
        }
    }
}
