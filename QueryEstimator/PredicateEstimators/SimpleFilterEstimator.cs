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
    public class SimpleFilterEstimator : BasePredicateEstimator<Dictionary<TableAttribute, int>, TableAttribute, IComparable>
    {
        public override Dictionary<TableAttribute, int> UpperBounds { get; }
        public override Dictionary<TableAttribute, int> LowerBounds { get; }

        public SimpleFilterEstimator(Dictionary<TableAttribute, int> upperBounds, Dictionary<TableAttribute, int> lowerBounds, IHistogramManager histogramManager) : base(histogramManager)
        {
            UpperBounds = upperBounds;
            LowerBounds = lowerBounds;
        }

        public override ISegmentResult GetEstimationResult(ISegmentResult current, TableAttribute source, IComparable compare, ComparisonType.Type type, bool isReverse = false)
        {
            var allSourceSegments = GetAllSegmentsForAttribute(source);
            int newSourceLowerBound = GetValueFromDictOrAlt(source, LowerBounds, 0);
            int newSourceUpperBound = GetValueFromDictOrAlt(source, UpperBounds, allSourceSegments.Count);

            var compType = compare.GetType();
            var valueType = allSourceSegments[newSourceLowerBound].LowestValue.GetType();
            if (compType != valueType)
                compare = (IComparable)Convert.ChangeType(compare, valueType);

            if (type == ComparisonType.Type.More)
            {
                for (int i = newSourceLowerBound; i < newSourceUpperBound; i++)
                {
                    if (allSourceSegments[i].LowestValue.IsLargerThanOrEqual(compare))
                        break;
                    newSourceLowerBound = i;
                }
            }
            if (type == ComparisonType.Type.Less)
            {
                for (int i = newSourceUpperBound - 1; i >= newSourceLowerBound; i--)
                {
                    if (allSourceSegments[i].LowestValue.IsLessThanOrEqual(compare))
                        break;
                    newSourceUpperBound = i;
                }
            }

            AddToDictionaryIfNotThere(source, newSourceUpperBound, UpperBounds);
            AddToDictionaryIfNotThere(source, newSourceLowerBound, LowerBounds);

            return current;
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
