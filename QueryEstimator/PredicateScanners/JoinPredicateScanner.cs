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

        public JoinPredicateScanner()
        {
            Predicates = new Dictionary<Type, List<IPredicate>>();
            _usedAttributes = new List<TableAttribute>();
        }

        public void Scan(List<JoinNode> nodes)
        {
            Predicates.Clear();
            _usedAttributes.Clear();

            List<FilterPredicate> baseFilters = new List<FilterPredicate>();
            List<TableAttribute> usedFilterAttributes = new List<TableAttribute>();

            // Scanning
            foreach (var node in nodes)
            {
                foreach (var predicate in node.Predicates)
                {
                    if (predicate.LeftAttribute.Attribute != null && predicate.RightAttribute.Attribute != null)
                    {
                        AddPredicateIfValid(predicate);
                        usedFilterAttributes.Add(predicate.LeftAttribute.Attribute);
                        usedFilterAttributes.Add(predicate.RightAttribute.Attribute);
                    }
                    else if (predicate.LeftAttribute.ConstantValue != null && predicate.RightAttribute.Attribute != null)
                    {
                        if (predicate.GetComType() == ComparisonType.Type.Less)
                            baseFilters.Add(new FilterPredicate(
                                predicate.RightAttribute.Attribute,
                                predicate.LeftAttribute.ConstantValue,
                                ComparisonType.Type.More));
                        if (predicate.GetComType() == ComparisonType.Type.More)
                            baseFilters.Add(new FilterPredicate(
                                predicate.RightAttribute.Attribute,
                                predicate.LeftAttribute.ConstantValue,
                                ComparisonType.Type.Less));
                    }
                    else if (predicate.LeftAttribute.Attribute != null && predicate.RightAttribute.ConstantValue != null)
                    {
                        baseFilters.Add(new FilterPredicate(
                            predicate.LeftAttribute.Attribute,
                            predicate.RightAttribute.ConstantValue,
                            predicate.GetComType()));
                    }
                    else
                        throw new Exception("Impossible predicate detected.");
                }
            }

            // Seperate filters
            foreach (var filter in baseFilters)
            {
                if (usedFilterAttributes.Contains(filter.LeftTable))
                    AddToDict<SimpleFilterPredicate>(new SimpleFilterPredicate(filter.LeftTable, filter.ConstantValue, filter.ComType));
                else
                    AddToDict<CrossFilterPredicate>(new CrossFilterPredicate(filter.LeftTable, filter.ConstantValue, filter.ComType));
            }
        }

        private void AddPredicateIfValid(JoinPredicate predicate)
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
                        if (predicate.GetComType() == ComparisonType.Type.Less)
                        {
                            AddToDict<TableAttributePredicate>(new TableAttributePredicate(
                                predicate.RightAttribute.Attribute,
                                predicate.LeftAttribute.Attribute,
                                ComparisonType.Type.More));
                        }
                        else if (predicate.GetComType() == ComparisonType.Type.More)
                        {
                            AddToDict<TableAttributePredicate>(new TableAttributePredicate(
                                predicate.RightAttribute.Attribute,
                                predicate.LeftAttribute.Attribute,
                                ComparisonType.Type.Less));
                        }
                        else
                            throw new Exception("Invalid Predicate!");
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
            }
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
