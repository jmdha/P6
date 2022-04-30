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
            int sourceUpperBound = GetUpperBoundOrAlt(source, allSourceSegments.Count);
            int compareLowerBound = GetLowerBoundOrAlt(compare, 0);
            int compareUpperBound = GetUpperBoundOrAlt(compare, allcompareSegments.Count);

            if (type == ComparisonType.Type.More || type == ComparisonType.Type.EqualOrMore)
            {
                long bottomBoundsOffset = 0;
                if (compareLowerBound == 0)
                    bottomBoundsOffset = (long)allcompareSegments[compareLowerBound].GetCountSmallerThanNoAlias(compare);
                else
                    bottomBoundsOffset = (long)allcompareSegments[compareLowerBound - 1].GetCountSmallerThanNoAlias(compare);
                for (int i = sourceUpperBound - 1; i >= sourceLowerBound; i--)
                {
                    if (doesPreviousContain)
                        newResult += (long)allSourceSegments[i].GetCountSmallerThanNoAlias(compare) - bottomBoundsOffset;
                    else
                        newResult += ((long)allSourceSegments[i].GetCountSmallerThanNoAlias(compare) - bottomBoundsOffset) * allSourceSegments[i].ElementsBeforeNextSegmentation;
                }
            } else if (type == ComparisonType.Type.Less || type == ComparisonType.Type.EqualOrLess)
            {
                long topBoundsOffset = 0;
                if (compareUpperBound == 0)
                    topBoundsOffset = (long)allcompareSegments[compareUpperBound].GetCountLargerThanNoAlias(compare);
                else
                    topBoundsOffset = (long)allcompareSegments[compareUpperBound - 1].GetCountLargerThanNoAlias(compare);
                for (int i = sourceLowerBound + 1; i < sourceUpperBound; i++)
                {
                    if (doesPreviousContain)
                        newResult += (long)allSourceSegments[i].GetCountLargerThanNoAlias(compare) - topBoundsOffset;
                    else
                        newResult += ((long)allSourceSegments[i].GetCountLargerThanNoAlias(compare) - topBoundsOffset) * allSourceSegments[i].ElementsBeforeNextSegmentation;
                }
            } else if (type == ComparisonType.Type.Equal)
            {
                long topBoundsOffset = 0;
                if (compareUpperBound == 0)
                    topBoundsOffset = (long)allcompareSegments[compareUpperBound].GetCountLargerThanNoAlias(compare);
                else
                    topBoundsOffset = (long)allcompareSegments[compareUpperBound - 1].GetCountLargerThanNoAlias(compare);
                IHistogramSegmentationComparative lastEqual = allSourceSegments[sourceLowerBound];
                for (int i = sourceLowerBound + 1; i < sourceUpperBound; i++)
                {
                    long aboveThis = (long)allSourceSegments[i].GetCountLargerThanNoAlias(compare) - topBoundsOffset;
                    long abovePreviousThis = (long)lastEqual.GetCountLargerThanNoAlias(compare) - topBoundsOffset;
                    if (doesPreviousContain)
                    {
                        newResult += abovePreviousThis - aboveThis;
                    }
                    else
                    {
                        newResult += (abovePreviousThis - aboveThis) * allSourceSegments[i].ElementsBeforeNextSegmentation;
                    }
                    lastEqual = allSourceSegments[i];
                }
            }

            return new ValueTableAttributeResult(UpperBounds[source], LowerBounds[source], source, UpperBounds[compare], LowerBounds[compare], compare, newResult, type);
        }
    }
}
