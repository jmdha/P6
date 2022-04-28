using Histograms;
using Histograms.Models;
using QueryEstimator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.Helpers;
using Tools.Models.JsonModels;

namespace QueryEstimator.PredicateEstimators
{
    public class FilterEstimator : BasePredicateEstimator<TableAttribute, IComparable>
    {
        public FilterEstimator(Dictionary<TableAttribute, int> upperBounds, Dictionary<TableAttribute, int> lowerBounds, IHistogramManager histogramManager) : base(upperBounds, lowerBounds, histogramManager)
        {
        }

        public override ISegmentResult GetEstimationResult(ISegmentResult current, TableAttribute source, IComparable compare, ComparisonType.Type type, bool isReverse = false)
        {
            var allSourceSegments = GetAllSegmentsForAttribute(source);

            return new SegmentResult(current, new ValueFilterResult(0, allSourceSegments.Count - 1, source, compare, type));
        }
    }
}
