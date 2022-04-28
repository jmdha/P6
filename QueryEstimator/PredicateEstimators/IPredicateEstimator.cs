using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.Models.JsonModels;

namespace QueryEstimator.PredicateEstimators
{
    public interface IPredicateEstimator<TBounds, RefT, TLeft, TRight>
    {
        public TBounds UpperBounds { get; }
        public TBounds LowerBounds { get; }

        public void GetEstimationResult(RefT dict, TLeft source, TRight compare, ComparisonType.Type type);
    }
}
