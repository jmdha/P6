using Histograms;
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
            int newSourceLowerBound = GetValueFromDictOrAlt(source, LowerBounds, 0);
            int newSourceUpperBound = GetValueFromDictOrAlt(source, UpperBounds, allSourceSegments.Count);

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

            return new ValueTableAttributeResult(UpperBounds[source], LowerBounds[source], source, UpperBounds[compare], LowerBounds[compare], compare, newResult, type);
        }

        internal void AddToDictionaryIfNotThere(TableAttribute attr, int bound, Dictionary<TableAttribute, int> dict)
        {
            if (dict.ContainsKey(attr))
                dict[attr] = bound;
            else
                dict.Add(attr, bound);
        }

        internal int GetValueFromDictOrAlt(TableAttribute attr, Dictionary<TableAttribute, int> dict, int alt)
        {
            if (dict.ContainsKey(attr))
                return dict[attr];
            return alt;
        }
    }
}
