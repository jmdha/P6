using Histograms;
using Histograms.Models;
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
    public abstract class BasePredicateBounder<TRight> : BaseSegmentBoundsHandler, IPredicateBounder<TRight>
    {
        protected BasePredicateBounder(Dictionary<TableAttribute, int> upperBounds, Dictionary<TableAttribute, int> lowerBounds, IHistogramManager histogramManager) : base(upperBounds, lowerBounds, histogramManager)
        {
        }

        public abstract IPredicateBoundResult<TRight> Bound(TableAttribute source, TRight compare, ComparisonType.Type type);
    }
}
