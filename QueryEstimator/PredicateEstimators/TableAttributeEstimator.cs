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
            int newSourceLowerBound = GetLowerBoundOrAlt(source, 0);
            int newSourceUpperBound = GetUpperBoundOrAlt(source, allSourceSegments.Count);

            if (type == ComparisonType.Type.More)
            {
                for (int i = newSourceUpperBound - 1; i >= newSourceLowerBound; i--)
                {
                    if (doesPreviousContain)
                        newResult += (long)allSourceSegments[i].GetCountSmallerThanNoAlias(compare);
                    else
                        newResult += (long)allSourceSegments[i].GetCountSmallerThanNoAlias(compare) * allSourceSegments[i].ElementsBeforeNextSegmentation;
                }
            }
            if (type == ComparisonType.Type.Less)
            {
                for (int i = newSourceLowerBound; i < newSourceUpperBound; i++)
                {
                    if (doesPreviousContain)
                        newResult += (long)allSourceSegments[i].GetCountLargerThanNoAlias(compare);
                    else
                        newResult += (long)allSourceSegments[i].GetCountLargerThanNoAlias(compare) * allSourceSegments[i].ElementsBeforeNextSegmentation;
                }
            }
            if (type == ComparisonType.Type.Equal)
            {
                IHistogramSegmentationComparative lastEqual = allSourceSegments[newSourceLowerBound];
                for (int i = newSourceLowerBound + 1; i < newSourceUpperBound; i++)
                {
                    if (doesPreviousContain)
                    {
                        newResult += (long)allSourceSegments[i].GetCountSmallerThanNoAlias(compare) - (long)lastEqual.GetCountSmallerThanNoAlias(compare);
                    }
                    else
                    {
                        newResult += ((long)allSourceSegments[i].GetCountSmallerThanNoAlias(compare) - (long)lastEqual.GetCountSmallerThanNoAlias(compare)) * allSourceSegments[i].ElementsBeforeNextSegmentation;
                    }
                    lastEqual = allSourceSegments[i];
                }
            }

            return new ValueTableAttributeResult(UpperBounds[source], LowerBounds[source], source, UpperBounds[compare], LowerBounds[compare], compare, newResult, type);
        }
    }
}
