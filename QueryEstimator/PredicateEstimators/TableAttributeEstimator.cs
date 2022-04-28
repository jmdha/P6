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
    public class TableAttributeEstimator : BasePredicateEstimator<Dictionary<TableAttribute, List<ISegmentResult?>>, TableAttribute, TableAttribute>
    {
        public TableAttributeEstimator(Dictionary<TableAttribute, int> upperBounds, Dictionary<TableAttribute, int> lowerBounds, IHistogramManager histogramManager) : base(upperBounds, lowerBounds, histogramManager)
        {
        }

        public override void GetEstimationResult(Dictionary<TableAttribute, List<ISegmentResult?>> dict, TableAttribute source, TableAttribute compare, ComparisonType.Type type)
        {
            var allSourceSegments = GetAllSegmentsForAttribute(source);
            int newSourceLowerBound = GetValueFromDictOrAlt(source, LowerBounds, 0);
            int newSourceUpperBound = GetValueFromDictOrAlt(source, UpperBounds, allSourceSegments.Count);
            bool foundAny = false;
            for (int i = newSourceLowerBound; i < newSourceUpperBound; i++)
            {
                ValueResult? newSegmentResult = null;
                if (type == ComparisonType.Type.More)
                    newSegmentResult = GetLargerCountTableAttributes(allSourceSegments[i], source, compare);
                if (type == ComparisonType.Type.Less)
                    newSegmentResult = GetSmallerCountTableAttributes(allSourceSegments[i], source, compare);
                if (newSegmentResult == null)
                    throw new ArgumentNullException();

                if (newSegmentResult.GetTotalEstimation() == 0 && foundAny)
                {
                    newSourceUpperBound = i;
                    AddNullToDict(dict, source, i);
                    break;
                }
                else if (newSegmentResult.GetTotalEstimation() == 0)
                {
                    newSourceLowerBound++;
                    AddNullToDict(dict, source, i);
                    continue;
                }
                else
                {
                    foundAny = true;

                    AddValueToDict(dict, source, compare, newSegmentResult, i);
                }
            }

            AddToDictionaryIfNotThere(source, newSourceUpperBound, UpperBounds);
            AddToDictionaryIfNotThere(source, newSourceLowerBound, LowerBounds);

            LimitOtherAttribute(dict, source, compare, type);
        }

        private void LimitOtherAttribute(Dictionary<TableAttribute, List<ISegmentResult?>> dict, TableAttribute source, TableAttribute compare, ComparisonType.Type type)
        {
            var allSourceSegments = GetAllSegmentsForAttribute(compare);
            int newSourceLowerBound = GetValueFromDictOrAlt(compare, LowerBounds, 0);
            int newSourceUpperBound = GetValueFromDictOrAlt(compare, UpperBounds, allSourceSegments.Count);
            bool foundAny = false;
            for (int i = newSourceLowerBound; i < newSourceUpperBound; i++)
            {
                ValueResult? newSegmentResult = null;
                if (type == ComparisonType.Type.More)
                    newSegmentResult = GetLargerCountTableAttributes(allSourceSegments[i], compare, source);
                if (type == ComparisonType.Type.Less)
                    newSegmentResult = GetSmallerCountTableAttributes(allSourceSegments[i], compare, source);
                if (newSegmentResult == null)
                    throw new ArgumentNullException();

                if (newSegmentResult.GetTotalEstimation() == 0 && foundAny)
                {
                    newSourceUpperBound = i;
                    AddNullToDict(dict, compare, i);
                    break;
                }
                else if (newSegmentResult.GetTotalEstimation() == 0)
                {
                    newSourceLowerBound++;
                    AddNullToDict(dict, compare, i);
                    continue;
                }
                else
                {
                    foundAny = true;
                }
            }

            AddToDictionaryIfNotThere(compare, newSourceUpperBound, UpperBounds);
            AddToDictionaryIfNotThere(compare, newSourceLowerBound, LowerBounds);
        }

        private void AddNullToDict(Dictionary<TableAttribute, List<ISegmentResult?>> dict, TableAttribute attr, int bound)
        {
            if (dict.ContainsKey(attr))
            {
                if (dict[attr].Count < bound)
                    dict[attr].Add(null);
            }
            else
            {
                dict.Add(attr, new List<ISegmentResult?>());
                dict[attr].Add(null);
            }
        }

        private void AddValueToDict(Dictionary<TableAttribute, List<ISegmentResult?>> dict, TableAttribute attr, TableAttribute compare, ValueResult newResult, int bound)
        {
            if (dict.ContainsKey(attr))
            {
                if (dict[attr].Count > bound && dict[attr][bound] is ISegmentResult seg)
                    dict[attr][bound] = new SegmentResult(seg, new ValueResult(attr, compare, newResult.LeftCount, 1));
                else
                    dict[attr].Insert(bound, newResult);
            }
            else
            {
                dict.Add(attr, new List<ISegmentResult?>());
                dict[attr].Insert(bound, newResult);
            }
        }
    }
}
