using QueryEstimator.Models;
using QueryEstimator.Models.BoundResults;
using QueryEstimator.SegmentHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.Models.JsonModels;

namespace QueryEstimator.PredicateBounders
{
    public interface IPredicateBounder<TRight> : ISegmentHandler
    {
        public IPredicateBoundResult<TRight> Bound(TableAttribute source, TRight compare, ComparisonType.Type type);
    }
}
