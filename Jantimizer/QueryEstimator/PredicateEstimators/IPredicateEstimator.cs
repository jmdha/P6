using QueryEstimator.Models;
using QueryEstimator.SegmentHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.Models.JsonModels;

namespace QueryEstimator.PredicateEstimators
{
    public interface IPredicateEstimator<TRight> : ISegmentHandler
    {
        public ISegmentResult GetEstimationResult(ISegmentResult current, TableAttribute source, TRight compare, ComparisonType.Type type);
    }
}
