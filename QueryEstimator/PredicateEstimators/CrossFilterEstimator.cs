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
    public class CrossFilterEstimator : BasePredicateEstimator<Dictionary<string, int>, TableAttribute, IComparable>
    {
        public override Dictionary<string, int> UpperBounds { get; }
        public override Dictionary<string, int> LowerBounds { get; }

        public CrossFilterEstimator(Dictionary<string, int> upperBounds, Dictionary<string, int> lowerBounds, IHistogramManager histogramManager) : base(histogramManager)
        {
            UpperBounds = upperBounds;
            LowerBounds = lowerBounds;
        }

        public override ISegmentResult GetEstimationResult(ISegmentResult current, TableAttribute source, IComparable compare, ComparisonType.Type type, bool isReverse = false)
        {
            var allSourceSegments = GetAllSegmentsForAttribute(source);
            int newSourceLowerBound = GetValueFromDictOrAlt(source.Table.TableName, LowerBounds, 0);
            int newSourceUpperBound = GetValueFromDictOrAlt(source.Table.TableName, UpperBounds, allSourceSegments.Count);

            if (type == ComparisonType.Type.More)
            {

            }
            if (type == ComparisonType.Type.Less)
            {

            }

            AddToDictionaryIfNotThere(source.Table.TableName, newSourceUpperBound, UpperBounds);
            AddToDictionaryIfNotThere(source.Table.TableName, newSourceLowerBound, LowerBounds);

            return new SegmentResult(current, new ValueFilterResult(0, allSourceSegments.Count - 1, source, compare, type));
        }

        internal void AddToDictionaryIfNotThere(string tableName, int bound, Dictionary<string, int> dict)
        {
            if (dict.ContainsKey(tableName))
                dict[tableName] = bound;
            else
                dict.Add(tableName, bound);
        }

        internal int GetValueFromDictOrAlt(string tableName, Dictionary<string, int> dict, int alt)
        {
            if (dict.ContainsKey(tableName))
                return dict[tableName];
            return alt;
        }
    }
}
