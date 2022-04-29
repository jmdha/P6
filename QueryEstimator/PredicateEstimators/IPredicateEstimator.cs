using Histograms;
using QueryEstimator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.Models.JsonModels;

namespace QueryEstimator.PredicateEstimators
{
    public interface IPredicateEstimator<TBounds, TLeft, TRight>
    {
        public TBounds UpperBounds { get; }
        public TBounds LowerBounds { get; }
        public IHistogramManager HistogramManager { get; }

        public ISegmentResult GetEstimationResult(ISegmentResult current, TLeft source, TRight compare, ComparisonType.Type type, bool isReverse = false);
    }
}
