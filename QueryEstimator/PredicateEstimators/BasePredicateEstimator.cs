using Histograms;
using Histograms.Models;
using QueryEstimator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.Models.JsonModels;

namespace QueryEstimator.PredicateEstimators
{
    public abstract class BasePredicateEstimator<TBound, TLeft, TRight> : IPredicateEstimator<TBound, TLeft, TRight>
    {
        public abstract TBound UpperBounds { get; }
        public abstract TBound LowerBounds { get; }
        public IHistogramManager HistogramManager { get; }

        protected BasePredicateEstimator(IHistogramManager histogramManager)
        {
            HistogramManager = histogramManager;
        }

        public abstract ISegmentResult GetEstimationResult(ISegmentResult current, TLeft source, TRight compare, ComparisonType.Type type, bool isReverse = false);

        internal List<IHistogramSegmentationComparative> GetAllSegmentsForAttribute(TableAttribute attr)
        {
            return HistogramManager.GetHistogram(attr.Table.TableName, attr.Attribute).Segmentations;
        }
    }
}
