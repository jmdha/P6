using QueryEstimator.Models.PredicateScanners;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.Models.JsonModels;

namespace QueryEstimator.PredicateScanners
{
    internal class JoinPredicateScanner : IPredicateScanner<List<JoinNode>>
    {
        public Dictionary<Type, List<IPredicate>> Predicates { get; }
        private List<TableAttribute> _usedAttributes;

        private List<FilterPredicate> _baseFilters;
        private List<TableAttribute> _usedFilterAttributes;

        public JoinPredicateScanner()
        {
            Predicates = new Dictionary<Type, List<IPredicate>>();
            _usedAttributes = new List<TableAttribute>();
            _baseFilters = new List<FilterPredicate>();
            _usedFilterAttributes = new List<TableAttribute>();
        }

        public void Scan(List<JoinNode> nodes)
        {
            Predicates.Clear();
            _usedAttributes.Clear();
            _baseFilters.Clear();
            _usedFilterAttributes.Clear();

            // Scanning
            foreach (var node in nodes)
            {
                foreach (var predicate in node.Predicates)
                {
                    bool foundAny = false;
                    foundAny = AddPredicateIfValid(predicate);
                    if (!foundAny)
                        foundAny = AddFilterIfValid(predicate);
                    if (!foundAny)
                        throw new Exception("Impossible predicate detected.");
                }
            }

            // Seperate filters
            foreach (var filter in _baseFilters)
            {
                if (_usedFilterAttributes.Contains(filter.LeftTable))
                    AddToDict<SimpleFilterPredicate>(new SimpleFilterPredicate(filter.LeftTable, filter.ConstantValue, filter.ComType));
                else
                    AddToDict<CrossFilterPredicate>(new CrossFilterPredicate(filter.LeftTable, filter.ConstantValue, filter.ComType));
            }
        }

        private bool AddFilterIfValid(JoinPredicate predicate)
        {
            if (predicate.LeftAttribute.ConstantValue != null && predicate.RightAttribute.Attribute != null)
            {
                switch (predicate.GetComType())
                {
                    case ComparisonType.Type.Equal:
                        _baseFilters.Add(new FilterPredicate(
                            predicate.RightAttribute.Attribute,
                            predicate.LeftAttribute.ConstantValue,
                            ComparisonType.Type.Equal));
                        break;
                    case ComparisonType.Type.Less:
                        _baseFilters.Add(new FilterPredicate(
                            predicate.RightAttribute.Attribute,
                            predicate.LeftAttribute.ConstantValue,
                            ComparisonType.Type.More));
                        break;
                    case ComparisonType.Type.More:
                        _baseFilters.Add(new FilterPredicate(
                            predicate.RightAttribute.Attribute,
                            predicate.LeftAttribute.ConstantValue,
                            ComparisonType.Type.Less));
                        break;
                    case ComparisonType.Type.EqualOrLess:
                        _baseFilters.Add(new FilterPredicate(
                            predicate.RightAttribute.Attribute,
                            predicate.LeftAttribute.ConstantValue,
                            ComparisonType.Type.EqualOrMore));
                        break;
                    case ComparisonType.Type.EqualOrMore:
                        _baseFilters.Add(new FilterPredicate(
                            predicate.RightAttribute.Attribute,
                            predicate.LeftAttribute.ConstantValue,
                            ComparisonType.Type.EqualOrLess));
                        break;
                    default:
                        throw new Exception("Invalid Predicate!");
                }
                return true;
            }
            else if (predicate.LeftAttribute.Attribute != null && predicate.RightAttribute.ConstantValue != null)
            {
                _baseFilters.Add(new FilterPredicate(
                    predicate.LeftAttribute.Attribute,
                    predicate.RightAttribute.ConstantValue,
                    predicate.GetComType()));
                return true;
            }
            return false;
        }

        private bool AddPredicateIfValid(JoinPredicate predicate)
        {
            if (predicate.LeftAttribute.Attribute != null && predicate.RightAttribute.Attribute != null)
            {
                if (_usedAttributes.Count == 0)
                {
                    AddToDict<TableAttributePredicate>(new TableAttributePredicate(
                        predicate.LeftAttribute.Attribute,
                        predicate.RightAttribute.Attribute,
                        predicate.GetComType()));
                    _usedAttributes.Add(predicate.LeftAttribute.Attribute);
                    _usedAttributes.Add(predicate.RightAttribute.Attribute);
                }
                else
                {
                    if (_usedAttributes.Contains(predicate.LeftAttribute.Attribute))
                    {
                        AddToDict<TableAttributePredicate>(new TableAttributePredicate(
                            predicate.LeftAttribute.Attribute,
                            predicate.RightAttribute.Attribute,
                            predicate.GetComType()));
                    }
                    else if (_usedAttributes.Contains(predicate.RightAttribute.Attribute))
                    {
                        switch (predicate.GetComType())
                        {
                            case ComparisonType.Type.Equal:
                                AddToDict<TableAttributePredicate>(new TableAttributePredicate(
                                    predicate.RightAttribute.Attribute,
                                    predicate.LeftAttribute.Attribute,
                                    ComparisonType.Type.Equal));
                                break;
                            case ComparisonType.Type.Less:
                                AddToDict<TableAttributePredicate>(new TableAttributePredicate(
                                    predicate.RightAttribute.Attribute,
                                    predicate.LeftAttribute.Attribute,
                                    ComparisonType.Type.More));
                                break;
                            case ComparisonType.Type.More:
                                AddToDict<TableAttributePredicate>(new TableAttributePredicate(
                                    predicate.RightAttribute.Attribute,
                                    predicate.LeftAttribute.Attribute,
                                    ComparisonType.Type.Less));
                                break;
                            case ComparisonType.Type.EqualOrLess:
                                AddToDict<TableAttributePredicate>(new TableAttributePredicate(
                                    predicate.RightAttribute.Attribute,
                                    predicate.LeftAttribute.Attribute,
                                    ComparisonType.Type.EqualOrMore));
                                break;
                            case ComparisonType.Type.EqualOrMore:
                                AddToDict<TableAttributePredicate>(new TableAttributePredicate(
                                    predicate.RightAttribute.Attribute,
                                    predicate.LeftAttribute.Attribute,
                                    ComparisonType.Type.EqualOrLess));
                                break;
                            default:
                                throw new Exception("Invalid Predicate!");
                        }
                    }
                    else
                    {
                        AddToDict<TableAttributePredicate>(new TableAttributePredicate(
                            predicate.LeftAttribute.Attribute,
                            predicate.RightAttribute.Attribute,
                            predicate.GetComType()));
                        _usedAttributes.Add(predicate.LeftAttribute.Attribute);
                    }
                }
                _usedFilterAttributes.Add(predicate.LeftAttribute.Attribute);
                _usedFilterAttributes.Add(predicate.RightAttribute.Attribute);
                return true;
            }
            return false;
        }

        public List<IPredicate> GetIfThere(Type t)
        {
            if (Predicates.ContainsKey(t))
                return Predicates[t];
            return new List<IPredicate>();
        }

        private void AddToDict<T>(IPredicate pred) where T : IPredicate
        {
            if (Predicates.ContainsKey(typeof(T)))
                Predicates[typeof(T)].Add(pred);
            else
                Predicates.Add(typeof(T), new List<IPredicate>() { pred });
        }
    }
}
