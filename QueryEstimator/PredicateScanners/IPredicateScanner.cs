using QueryEstimator.Models.PredicateScanners;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.Models.JsonModels;

namespace QueryEstimator.PredicateScanners
{
    public interface IPredicateScanner<T>
    {
        public Dictionary<Type, List<IPredicate>> Predicates { get; }
        public void Scan(T input);
        public List<IPredicate> GetIfThere(Type t);
    }
}
