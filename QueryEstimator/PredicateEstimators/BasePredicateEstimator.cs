using Histograms;
using Histograms.Models;
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
    public abstract class BasePredicateEstimator<TRight> : BaseSegmentBoundsHandler, IPredicateEstimator<TRight>
    {
        protected BasePredicateEstimator(Dictionary<TableAttribute, int> upperBounds, Dictionary<TableAttribute, int> lowerBounds, IHistogramManager histogramManager) : base(upperBounds, lowerBounds, histogramManager)
        {
        }

        public abstract ISegmentResult GetEstimationResult(ISegmentResult current, TableAttribute source, TRight compare, ComparisonType.Type type);
    }
}
