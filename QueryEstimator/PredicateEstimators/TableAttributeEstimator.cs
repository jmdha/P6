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
    public class TableAttributeEstimator : BasePredicateEstimator<Dictionary<TableAttribute, int>, TableAttribute, TableAttribute>
    {
        public override Dictionary<TableAttribute, int> UpperBounds { get; }
        public override Dictionary<TableAttribute, int> LowerBounds { get; }

        public TableAttributeEstimator(Dictionary<TableAttribute, int> upperBounds, Dictionary<TableAttribute, int> lowerBounds, IHistogramManager histogramManager) : base(histogramManager)
        {
            UpperBounds = upperBounds;
            LowerBounds = lowerBounds;
        }

        public override ISegmentResult GetEstimationResult(ISegmentResult current, TableAttribute source, TableAttribute compare, ComparisonType.Type type, bool isReverse = false)
        {
            long newResult = 0;

            var allSourceSegments = GetAllSegmentsForAttribute(source);
            int newSourceLowerBound = GetValueFromDictOrAlt(source, LowerBounds, 0);
            int newSourceUpperBound = GetValueFromDictOrAlt(source, UpperBounds, allSourceSegments.Count);

            if (type == ComparisonType.Type.More)
            {
                bool foundAny = false;
                for (int i = newSourceUpperBound - 1; i >= newSourceLowerBound; i--)
                {
                    var newInnerResult = (long)allSourceSegments[i].GetCountSmallerThanNoAlias(compare) * allSourceSegments[i].ElementsBeforeNextSegmentation;

                    if (newInnerResult == 0 && !foundAny)
                    {
                        newSourceUpperBound = i;
                        continue;
                    }
                    else if (newInnerResult == 0)
                    {
                        newSourceLowerBound = i;
                        break;
                    }
                    else
                    {
                        newResult += newInnerResult;
                        foundAny = true;
                    }
                }
                if (!isReverse)
                    GetEstimationResult(current, compare, source, ComparisonType.Type.Less, true);
            }
            if (type == ComparisonType.Type.Less)
            {
                bool foundAny = false;
                for (int i = newSourceLowerBound; i < newSourceUpperBound; i++)
                {
                    var newInnerResult = (long)allSourceSegments[i].GetCountLargerThanNoAlias(compare) * allSourceSegments[i].ElementsBeforeNextSegmentation;

                    if (newInnerResult == 0 && foundAny)
                    {
                        newSourceUpperBound = i;
                        break;
                    }
                    else if (newInnerResult == 0)
                    {
                        newSourceLowerBound = i;
                        continue;
                    }
                    else
                    {
                        newResult += newInnerResult;
                        foundAny = true;
                    }
                }
                if (!isReverse)
                    GetEstimationResult(current, compare, source, ComparisonType.Type.More, true);
            }

            AddToDictionaryIfNotThere(source, newSourceUpperBound, UpperBounds);
            AddToDictionaryIfNotThere(source, newSourceLowerBound, LowerBounds);

            return new SegmentResult(current, new ValueTableAttributeResult(newSourceUpperBound, newSourceLowerBound, source, compare, newResult, type));
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
