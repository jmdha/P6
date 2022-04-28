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
    public class FilterEstimator : BasePredicateEstimator<Dictionary<TableAttribute, List<ISegmentResult>>, TableAttribute, IComparable>
    {
        public FilterEstimator(Dictionary<TableAttribute, int> upperBounds, Dictionary<TableAttribute, int> lowerBounds, IHistogramManager histogramManager) : base(upperBounds, lowerBounds, histogramManager)
        {
        }

        public override void GetEstimationResult(Dictionary<TableAttribute, List<ISegmentResult>> dict, TableAttribute source, IComparable compare, ComparisonType.Type type)
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

                if (dict.ContainsKey(source))
                {
                    if (dict[source][i] != null)
                        dict[source][i] = new SegmentResult(dict[source][i], new ValueResult(source, source, newSegmentResult.LeftCount, 1));
                    else
                        dict[source].Insert(i, newSegmentResult);
                }
                else
                {
                    dict.Add(source, new List<ISegmentResult>());
                    dict[source].Insert(i, newSegmentResult);
                }

                if (newSegmentResult.GetTotalEstimation() == 0 && foundAny)
                {
                    newSourceUpperBound = i;
                    break;
                }
                else if (newSegmentResult.GetTotalEstimation() == 0)
                    newSourceLowerBound++;
                else
                    foundAny = true;
            }

            AddToDictionaryIfNotThere(source, newSourceUpperBound, UpperBounds);
            AddToDictionaryIfNotThere(source, newSourceLowerBound, LowerBounds);
        }

        private ValueResult GetLargerCountFilters(IHistogramSegmentationComparative source, TableAttribute fromAttr, IComparable compareValue)
        {
            if (compareValue.IsLessThanOrEqual(source.LowestValue))
            {
                return new ValueResult(
                    fromAttr,
                    fromAttr,
                    GetSmallerCountTableAttributes(source, fromAttr, fromAttr).GetTotalEstimation(),
                    source.ElementsBeforeNextSegmentation);
            }
            else
                return new ValueResult(
                    fromAttr,
                    fromAttr,
                    GetLargerCountTableAttributes(source, fromAttr, fromAttr).GetTotalEstimation(),
                    source.ElementsBeforeNextSegmentation);
        }

        private ValueResult GetSmallerCountFilters(IHistogramSegmentationComparative source, TableAttribute fromAttr, IComparable compareValue)
        {
            if (compareValue.IsLargerThanOrEqual(source.LowestValue))
            {
                return new ValueResult(
                    fromAttr,
                    fromAttr,
                    GetLargerCountTableAttributes(source, fromAttr, fromAttr).GetTotalEstimation(),
                    source.ElementsBeforeNextSegmentation);
            }
            else
                return new ValueResult(
                    fromAttr,
                    fromAttr,
                    GetSmallerCountTableAttributes(source, fromAttr, fromAttr).GetTotalEstimation(),
                    source.ElementsBeforeNextSegmentation);
        }
    }
}
