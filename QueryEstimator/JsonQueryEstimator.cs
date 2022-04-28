using Histograms;
using Histograms.Models;
using QueryEstimator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.Models.JsonModels;
using Tools.Helpers;

namespace QueryEstimator
{
    public class JsonQueryEstimator : IQueryEstimator<JsonQuery>
    {
        public IHistogramManager HistogramManager { get; }
        private Dictionary<TableAttribute, int> _upperBounds;
        private Dictionary<TableAttribute, int> _lowerBounds;
        private int _milestoneCeil = 1;

        public JsonQueryEstimator(IHistogramManager histogramManager)
        {
            HistogramManager = histogramManager;
            _upperBounds = new Dictionary<TableAttribute, int>();
            _lowerBounds = new Dictionary<TableAttribute, int>();
        }

        public EstimatorResult GetQueryEstimation(JsonQuery query)
        {
            _upperBounds.Clear();
            _lowerBounds.Clear();

            Dictionary<TableAttribute, List<ISegmentResult>> intermediateResults = new Dictionary<TableAttribute, List<ISegmentResult>>();
            foreach (var node in query.JoinNodes)
            {
                foreach (var predicate in node.Predicates)
                {
                    if (predicate.LeftAttribute.Attribute != null && predicate.RightAttribute.Attribute != null)
                    {
                        GetEstimationResultForPredicates(
                            intermediateResults,
                            predicate.LeftAttribute.Attribute,
                            predicate.RightAttribute.Attribute,
                            predicate.GetComType());
                    }
                    else if (predicate.LeftAttribute.ConstantValue != null && predicate.RightAttribute.Attribute != null)
                    {
                        if (predicate.GetComType() == ComparisonType.Type.Less)
                            GetEstimationResultForFilters(
                                intermediateResults,
                                predicate.RightAttribute.Attribute,
                                predicate.LeftAttribute.ConstantValue,
                                ComparisonType.Type.More);
                        if (predicate.GetComType() == ComparisonType.Type.More)
                            GetEstimationResultForFilters(
                                intermediateResults,
                                predicate.RightAttribute.Attribute,
                                predicate.LeftAttribute.ConstantValue,
                                ComparisonType.Type.Less);
                    }
                    else if (predicate.LeftAttribute.Attribute != null && predicate.RightAttribute.ConstantValue != null)
                    {
                        if (predicate.GetComType() == ComparisonType.Type.Less)
                            GetEstimationResultForFilters(
                                intermediateResults,
                                predicate.LeftAttribute.Attribute,
                                predicate.RightAttribute.ConstantValue,
                                ComparisonType.Type.More);
                        if (predicate.GetComType() == ComparisonType.Type.More)
                            GetEstimationResultForFilters(
                                intermediateResults,
                                predicate.LeftAttribute.Attribute,
                                predicate.RightAttribute.ConstantValue,
                                ComparisonType.Type.Less);
                    }
                    else
                        throw new Exception("Impossible predicate detected.");
                }
            }

            long returnValue = 0;

            foreach (var value in StitchListDicts(intermediateResults))
            {
                returnValue += value.GetTotalEstimation();
            }

            return new EstimatorResult(query, (ulong)returnValue);
        }

        private void GetEstimationResultForPredicates(Dictionary<TableAttribute, List<ISegmentResult>> current, TableAttribute from, TableAttribute compareAttr, ComparisonType.Type type)
        {
            var allSourceSegments = GetAllSegmentsForAttribute(from);
            if (allSourceSegments.Count > _milestoneCeil)
                _milestoneCeil = allSourceSegments.Count;
            int newSourceLowerBound = GetValueFromDictOrAlt(from, _lowerBounds, 0);
            int newSourceUpperBound = GetValueFromDictOrAlt(from, _upperBounds, allSourceSegments.Count);
            bool foundAny = false;
            for (int i = newSourceLowerBound; i < newSourceUpperBound; i++)
            {
                ValueResult? newSegmentResult = null;
                if (type == ComparisonType.Type.More)
                    newSegmentResult = GetLargerCountTableAttributes(allSourceSegments[i], from, compareAttr);
                if (type == ComparisonType.Type.Less)
                    newSegmentResult = GetSmallerCountTableAttributes(allSourceSegments[i], from, compareAttr);
                if (newSegmentResult == null)
                    throw new ArgumentNullException();

                if (current.ContainsKey(from))
                {
                    current[from].Add(newSegmentResult);
                }
                else
                {
                    current.Add(from, new List<ISegmentResult>());
                    current[from].Add(newSegmentResult);
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

            AddToDictionaryIfNotThere(from, newSourceUpperBound, _upperBounds);
            AddToDictionaryIfNotThere(from, newSourceLowerBound, _lowerBounds);
        }

        private void GetEstimationResultForFilters(Dictionary<TableAttribute, List<ISegmentResult>> current, TableAttribute from, IComparable constant, ComparisonType.Type type)
        {
            var allSourceSegments = GetAllSegmentsForAttribute(from);
            if (allSourceSegments.Count > _milestoneCeil)
                _milestoneCeil = allSourceSegments.Count;
            int newSourceLowerBound = GetValueFromDictOrAlt(from, _lowerBounds, 0);
            int newSourceUpperBound = GetValueFromDictOrAlt(from, _upperBounds, allSourceSegments.Count);
            bool foundAny = false;
            for (int i = newSourceLowerBound; i < newSourceUpperBound; i++)
            {
                ValueResult? newSegmentResult = null;
                if (type == ComparisonType.Type.More)
                    newSegmentResult = GetLargerCountFilters(allSourceSegments[i], from, constant);
                if (type == ComparisonType.Type.Less)
                    newSegmentResult = GetSmallerCountFilters(allSourceSegments[i], from, constant);
                if (newSegmentResult == null)
                    throw new ArgumentNullException();

                if (current.ContainsKey(from))
                {
                    current[from].Add(newSegmentResult);
                }
                else
                {
                    current.Add(from, new List<ISegmentResult>());
                    current[from].Add(newSegmentResult);
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

            AddToDictionaryIfNotThere(from, newSourceUpperBound, _upperBounds);
            AddToDictionaryIfNotThere(from, newSourceLowerBound, _lowerBounds);
        }

        private List<ISegmentResult> StitchListDicts(Dictionary<TableAttribute, List<ISegmentResult>> dict)
        {
            var newList = new List<ISegmentResult?>();

            if (dict.Keys.Count > 0)
            {
                var firstKey = dict.Keys.First();
                int sourceLowerBound = GetValueFromDictOrAlt(firstKey, _lowerBounds, 0);
                int sourceUpperBound = GetValueFromDictOrAlt(firstKey, _upperBounds, dict[firstKey].Count);

                for (int i = 0; i < _milestoneCeil; i++)
                {
                    if (i >= sourceLowerBound && i < sourceUpperBound)
                        newList.Insert(i, dict[firstKey][i]);
                    if (i > sourceUpperBound)
                        break;
                }


                foreach (var key in dict.Keys.Skip(1))
                {
                    sourceLowerBound = GetValueFromDictOrAlt(key, _lowerBounds, 0);
                    sourceUpperBound = GetValueFromDictOrAlt(key, _upperBounds, dict[key].Count);

                    for (int i = 0; i < newList.Count; i++)
                    {
                        if (i >= sourceLowerBound && i < sourceUpperBound)
                        {
                            if (newList[i] != null)
                            {
                                if (newList[i].IsReferencingTableAttribute(key))
                                {
                                    var thisValue = dict[key][i];
                                    if (thisValue is ValueResult res)
                                        newList[i] = new SegmentResult(newList[i], new ValueResult(
                                            res.TableA,
                                            res.TableB,
                                            res.LeftCount,
                                            1));
                                }
                                else
                                {
                                    var thisValue = dict[key][i];
                                    if (thisValue is ValueResult res)
                                        newList[i] = new SegmentResult(newList[i], new ValueResult(
                                            res.TableA,
                                            res.TableB,
                                            res.LeftCount,
                                            res.RightCount));
                                }
                            }
                        }
                        if (i > sourceUpperBound)
                            break;
                    }
                }
            }

            return newList;
        }

        private ValueResult GetLargerCountTableAttributes(IHistogramSegmentationComparative source, TableAttribute fromAttr, TableAttribute compareAttr)
        {
            return new ValueResult(
                fromAttr,
                compareAttr,
                (long)source.CountLargerThan[compareAttr],
                source.ElementsBeforeNextSegmentation);
        }

        private ValueResult GetSmallerCountTableAttributes(IHistogramSegmentationComparative source, TableAttribute fromAttr, TableAttribute compareAttr)
        {
            return new ValueResult(
                fromAttr,
                compareAttr,
                (long)source.CountSmallerThan[compareAttr],
                source.ElementsBeforeNextSegmentation);
        }

        private ValueResult GetLargerCountFilters(IHistogramSegmentationComparative source, TableAttribute fromAttr, IComparable compareValue)
        {
            if (compareValue.IsLessThanOrEqual(source.LowestValue)) {
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

        private List<IHistogramSegmentationComparative> GetAllSegmentsForAttribute(TableAttribute attr)
        {
            var newList = new List<IHistogramSegmentationComparative>();
            return newList;
        }

        private long SumList(List<long> values)
        {
            long totalEstimate = 1;
            foreach (long value in values)
                totalEstimate += value;
            return totalEstimate;
        }

        private void AddToDictionaryIfNotThere(TableAttribute attr, int bound, Dictionary<TableAttribute,int> dict)
        {
            if (dict.ContainsKey(attr))
                dict[attr] = bound;
            else
                dict.Add(attr, bound);
        }

        private int GetValueFromDictOrAlt(TableAttribute attr, Dictionary<TableAttribute, int> dict, int alt)
        {
            if (dict.ContainsKey(attr))
                return dict[attr];
            return alt;
        }

        private List<CompareAttribute> GetOverlappingAttributes(List<JoinNode> nodes)
        {
            var returnList = new List<CompareAttribute>();
            var newList = new List<TableAttribute>();
            foreach(var node in nodes)
            {
                foreach (var pred in node.Predicates)
                {
                    if (pred.LeftAttribute.IsAttribute && pred.LeftAttribute.Attribute != null)
                    {
                        if (newList.Contains(pred.LeftAttribute.Attribute))
                            returnList.Add(new CompareAttribute(pred.LeftAttribute.Attribute, pred.GetComType()));
                        newList.Add(pred.LeftAttribute.Attribute);
                    }
                    if (pred.RightAttribute.IsAttribute && pred.RightAttribute.Attribute != null)
                    {
                        if (newList.Contains(pred.RightAttribute.Attribute))
                            returnList.Add(new CompareAttribute(pred.RightAttribute.Attribute, pred.GetComType()));
                        newList.Add(pred.RightAttribute.Attribute);
                    }
                }
            }
            return returnList;
        }
    }
}
