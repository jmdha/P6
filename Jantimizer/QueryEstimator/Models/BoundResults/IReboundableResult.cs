using QueryEstimator.PredicateBounders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.Models.JsonModels;

namespace QueryEstimator.Models.BoundResults
{
    public interface IReboundableResult<TRight> : IBoundResult
    {
        public IPredicateBounder<TRight> Bounder { get; }
        public TRight Right { get; }
        public ComparisonType.Type ComType { get; }

        public void RecalculateBounds();
    }
}
