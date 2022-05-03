using QueryEstimator.Exceptions;
using QueryEstimator.Helpers;
using QueryEstimator.Models.PredicateScanners;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Tools.Models.JsonModels;

[assembly: InternalsVisibleTo("QueryEstimatorTests")]

namespace QueryEstimator.PredicateScanners
{
    public class JoinPredicateScanner : IPredicateScanner<List<JoinNode>>
    {
        public Dictionary<Type, List<IPredicate>> Predicates { get; }
        internal List<TableAttribute> _usedAttributes;
        internal List<FilterPredicate> _baseFilters;

        public JoinPredicateScanner()
        {
            Predicates = new Dictionary<Type, List<IPredicate>>();
            _usedAttributes = new List<TableAttribute>();
            _baseFilters = new List<FilterPredicate>();
        }

        public void Scan(List<JoinNode> nodes)
        {
            Predicates.Clear();
            _usedAttributes.Clear();
            _baseFilters.Clear();

            // Scan for both predicates and filters
            foreach (var node in nodes)
            {
                foreach (var predicate in node.Predicates)
                {
                    bool foundAny = false;
                    foundAny = AddPredicateIfValid(predicate);
                    if (!foundAny)
                        foundAny = AddFilterIfValid(predicate);
                    if (!foundAny)
                        throw new PredicateScannerException("Impossible predicate detected.", PredicateScannerErrorType.Unscannable);
                }
            }

            if (_usedAttributes.Count == 0)
                throw new PredicateScannerException("No predicates in the join node!", PredicateScannerErrorType.NoTableAttributePrediacte);

            // Make sure filter attributes are used in the predicates
            foreach (var filter in _baseFilters)
            {
                if (!_usedAttributes.Contains(filter.LeftTable))
                    throw new PredicateScannerException("Invalid filter! Cannot have a filter that is not included in any of the JOIN predicates!", PredicateScannerErrorType.IlligalFilter);
                AddToDict<FilterPredicate>(new FilterPredicate(filter.LeftTable, filter.ConstantValue, filter.ComType));
            }
        }

        internal bool AddFilterIfValid(JoinPredicate predicate)
        {
            // Check if the filter is inverted, e.g. "50 < a.v"
            // If it is, add it but turn the order back to normal
            if (predicate.LeftAttribute.ConstantValue != null && predicate.RightAttribute.Attribute != null)
            {
                _baseFilters.Add(new FilterPredicate(
                    predicate.RightAttribute.Attribute,
                    predicate.LeftAttribute.ConstantValue,
                    ComparisonTypeHelper.InvertType(predicate.GetComType())));
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

        internal bool AddPredicateIfValid(JoinPredicate predicate)
        {
            if (predicate.LeftAttribute.Attribute != null && predicate.RightAttribute.Attribute != null)
            {
                if (_usedAttributes.Count > 0 && !_usedAttributes.Contains(predicate.LeftAttribute.Attribute) && !_usedAttributes.Contains(predicate.RightAttribute.Attribute))
                    return false;

                if (_usedAttributes.Contains(predicate.LeftAttribute.Attribute) && !_usedAttributes.Contains(predicate.RightAttribute.Attribute))
                {
                    AddToDict<TableAttributePredicate>(new TableAttributePredicate(
                        predicate.RightAttribute.Attribute,
                        predicate.LeftAttribute.Attribute,
                        ComparisonTypeHelper.InvertType(predicate.GetComType())));
                    _usedAttributes.Add(predicate.RightAttribute.Attribute);
                } 
                else
                {
                    AddToDict<TableAttributePredicate>(new TableAttributePredicate(
                        predicate.LeftAttribute.Attribute,
                        predicate.RightAttribute.Attribute,
                        predicate.GetComType()));
                    _usedAttributes.Add(predicate.LeftAttribute.Attribute);
                    _usedAttributes.Add(predicate.RightAttribute.Attribute);
                }
                return true;
            }
            return false;
        }

        public List<Pred> GetIfThere<Pred>() where Pred : IPredicate
        {
            var retList = new List<Pred>();
            if (Predicates.ContainsKey(typeof(Pred)))
            {
                var predList = Predicates[typeof(Pred)];
                foreach (var item in predList)
                    if (item is Pred accItem)
                        retList.Add(accItem);
            }
            return retList;
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
