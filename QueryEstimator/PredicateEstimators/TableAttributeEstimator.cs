using QueryEstimator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.Models.JsonModels;

namespace QueryEstimator.PredicateEstimators
{
    public class TableAttributeEstimator : BasePredicateEstimator<Dictionary<TableAttribute, List<ISegmentResult>>, TableAttribute, TableAttribute>
    {
        public TableAttributeEstimator(Dictionary<TableAttribute, int> upperBounds, Dictionary<TableAttribute, int> lowerBounds) : base(upperBounds, lowerBounds)
        {
        }

        public override void GetEstimationResult(Dictionary<TableAttribute, List<ISegmentResult>> dict, TableAttribute source, TableAttribute compare, ComparisonType.Type type)
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

                if (dict.ContainsKey(source))
                {
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
    }
}
