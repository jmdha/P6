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

            long bottomBoundsCount = GetBottomBoundsOffsetCount(allcompareSegments, compareLowerBound, compare);
            long topBoundsCount = GetTopBoundsOffsetCount(allcompareSegments, compareUpperBound, compare);

            IHistogramSegmentationComparative lastEqual = allSourceSegments[sourceLowerBound];
            if (type == ComparisonType.Type.Equal)
                sourceLowerBound++;

            for (int i = sourceLowerBound; i <= sourceUpperBound; i++)
            {
                switch (type)
                {
                    case ComparisonType.Type.More:
                    case ComparisonType.Type.EqualOrMore:
                        newResult += AddSegmentResult(
                            allSourceSegments[i],
                            (long)allSourceSegments[i].GetCountSmallerThanNoAlias(compare),
                            doesPreviousContain, bottomBoundsCount, topBoundsCount);
                        break;
                    case ComparisonType.Type.Less:
                    case ComparisonType.Type.EqualOrLess:
                        newResult += AddSegmentResult(
                            allSourceSegments[i],
                            (long)allSourceSegments[i].GetCountLargerThanNoAlias(compare),
                            doesPreviousContain, bottomBoundsCount, topBoundsCount);
                        break;
                    case ComparisonType.Type.Equal:
                        long aboveThis = (long)allSourceSegments[i].GetCountLargerThanNoAlias(compare);
                        long abovePreviousThis = (long)lastEqual.GetCountLargerThanNoAlias(compare);
                        aboveThis = AddSegmentResult(allSourceSegments[i], aboveThis, true, bottomBoundsCount, aboveThis);
                        abovePreviousThis = AddSegmentResult(lastEqual, abovePreviousThis, true, bottomBoundsCount, abovePreviousThis);
                        if (doesPreviousContain)
                            newResult += abovePreviousThis - aboveThis;
                        else
                            newResult += (abovePreviousThis - aboveThis) * allSourceSegments[i].ElementsBeforeNextSegmentation;
                        lastEqual = allSourceSegments[i];
                        break;
                }
            }

            return new ValueTableAttributeResult(UpperBounds[source], LowerBounds[source], source, UpperBounds[compare], LowerBounds[compare], compare, newResult, type);
        }

        private long GetBottomBoundsOffsetCount(List<IHistogramSegmentationComparative> segments, int compareIndex, TableAttribute compare)
        {
            return (long)segments[compareIndex].GetCountSmallerThanNoAlias(compare);
        }

        private long GetTopBoundsOffsetCount(List<IHistogramSegmentationComparative> segments, int compareIndex, TableAttribute compare)
        {
            return (long)segments[compareIndex].GetCountSmallerThanNoAlias(compare);
        }

        private long AddSegmentResult(IHistogramSegmentationComparative segment, long add, bool doesPreviousContain, long bottomOffsetCount, long checkOffsetCount)
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
