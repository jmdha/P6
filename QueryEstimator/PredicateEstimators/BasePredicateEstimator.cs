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
    public abstract class BasePredicateEstimator<RefT, TLeft, TRight> : IPredicateEstimator<
        Dictionary<TableAttribute, int>, 
        RefT, 
        TLeft, 
        TRight>
    {
        public Dictionary<TableAttribute, int> UpperBounds { get; }
        public Dictionary<TableAttribute, int> LowerBounds { get; }

        protected BasePredicateEstimator(Dictionary<TableAttribute, int> upperBounds, Dictionary<TableAttribute, int> lowerBounds)
        {
            UpperBounds = upperBounds;
            LowerBounds = lowerBounds;
        }

        public abstract void GetEstimationResult(RefT dict, TLeft source, TRight compare, ComparisonType.Type type);

        internal ValueResult GetLargerCountTableAttributes(IHistogramSegmentationComparative source, TableAttribute fromAttr, TableAttribute compareAttr)
        {
            return new ValueResult(
                fromAttr,
                compareAttr,
                (long)source.CountLargerThan[compareAttr],
                source.ElementsBeforeNextSegmentation);
        }

        internal ValueResult GetSmallerCountTableAttributes(IHistogramSegmentationComparative source, TableAttribute fromAttr, TableAttribute compareAttr)
        {
            return new ValueResult(
                fromAttr,
                compareAttr,
                (long)source.CountSmallerThan[compareAttr],
                source.ElementsBeforeNextSegmentation);
        }

        internal List<IHistogramSegmentationComparative> GetAllSegmentsForAttribute(TableAttribute attr)
        {
            var newList = new List<IHistogramSegmentationComparative>();
            return newList;
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
