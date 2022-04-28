using Histograms;
using Histograms.Models;
using QueryEstimator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.Helpers;
using Tools.Models.JsonModels;

namespace QueryEstimator.PredicateEstimators
{
    public class FilterEstimator : BasePredicateEstimator<Dictionary<TableAttribute, List<ISegmentResult?>>, TableAttribute, IComparable>
    {
        public FilterEstimator(Dictionary<TableAttribute, int> upperBounds, Dictionary<TableAttribute, int> lowerBounds, IHistogramManager histogramManager) : base(upperBounds, lowerBounds, histogramManager)
        {
        }

        public override void GetEstimationResult(Dictionary<TableAttribute, List<ISegmentResult?>> dict, TableAttribute source, IComparable compare, ComparisonType.Type type)
        {
            var allSourceSegments = GetAllSegmentsForAttribute(source);
            int newSourceLowerBound = GetValueFromDictOrAlt(source, LowerBounds, 0);
            int newSourceUpperBound = GetValueFromDictOrAlt(source, UpperBounds, allSourceSegments.Count);
            bool foundAny = false;
            for (int i = newSourceLowerBound; i < newSourceUpperBound; i++)
            {
                ValueResult? newSegmentResult = null;
                if (type == ComparisonType.Type.More)
                    newSegmentResult = GetLargerCountFilters(allSourceSegments[i], source, compare);
                if (type == ComparisonType.Type.Less)
                    newSegmentResult = GetSmallerCountFilters(allSourceSegments[i], source, compare);
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
                    AddValueToDict(dict, source, newSegmentResult, i);
                }
            }

            AddToDictionaryIfNotThere(source, newSourceUpperBound, UpperBounds);
            AddToDictionaryIfNotThere(source, newSourceLowerBound, LowerBounds);
        }

        private ValueResult GetLargerCountFilters(IHistogramSegmentationComparative source, TableAttribute fromAttr, IComparable compareValue)
        {
            var targetType = compareValue.GetType();
            var valueType = source.LowestValue.GetType();
            if (targetType != valueType)
            {
                compareValue = (IComparable)Convert.ChangeType(compareValue, valueType);
            }

            if (compareValue.IsLessThanOrEqual(source.LowestValue))
            {
                return new ValueResult(
                    fromAttr,
                    fromAttr,
                    1,
                    source.ElementsBeforeNextSegmentation);
            }
            else
                return new ValueResult(
                    fromAttr,
                    fromAttr,
                    0,
                    0);
        }

        private ValueResult GetSmallerCountFilters(IHistogramSegmentationComparative source, TableAttribute fromAttr, IComparable compareValue)
        {
            var targetType = compareValue.GetType();
            var valueType = source.LowestValue.GetType();
            if (targetType != valueType)
            {
                compareValue = (IComparable)Convert.ChangeType(compareValue, valueType);
            }

            if (compareValue.IsLargerThanOrEqual(source.LowestValue))
            {
                return new ValueResult(
                    fromAttr,
                    fromAttr,
                    1,
                    source.ElementsBeforeNextSegmentation);
            }
            else
                return new ValueResult(
                    fromAttr,
                    fromAttr,
                    0,
                    0);
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

        private void AddValueToDict(Dictionary<TableAttribute, List<ISegmentResult?>> dict, TableAttribute attr, ValueResult newResult, int bound)
        {
            if (dict.ContainsKey(attr))
            {
                if (dict[attr].Count > bound && dict[attr][bound] is ISegmentResult seg)
                    dict[attr][bound] = new SegmentResult(seg, new ValueResult(attr, attr, newResult.LeftCount, 1));
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
