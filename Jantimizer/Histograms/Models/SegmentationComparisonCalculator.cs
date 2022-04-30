using Histograms.DataGatherers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.Helpers;
using Tools.Models.JsonModels;

namespace Histograms.Models
{
    public class SegmentationComparisonCalculator
    {
        protected IDataGatherer DataGatherer { get; set; }
        public SegmentationComparisonCalculator(IDataGatherer dataGatherer)
        {
            DataGatherer = dataGatherer;
        }

        public async Task DoHistogramComparisons(IEnumerable<IHistogram> histograms)
        {
            IEnumerable<TableAttribute> attributes = GetAllDistinctTableAttributes(histograms);

            foreach (TableAttribute attribute in attributes)
            {
                DoAllComparisonsForAttribute(histograms, await DataGatherer.GetData(attribute));
            }
        }



        private void DoAllComparisonsForAttribute(IEnumerable<IHistogram> histograms, AttributeData data)
        {
            foreach (IHistogram histogram in histograms)
            {
                if (histogram.DataTypeCode != data.TypeCode)
                    continue;

                foreach(var segmentation in histogram.Segmentations)
                {
                    CalculateAndInsertComparisons(segmentation, data);
                }
            }
        }

        private void CalculateAndInsertComparisons(IHistogramSegmentationComparative segmentation, AttributeData data)
        {
            ulong smaller = 0;
            ulong larger = 0;

            foreach(var valueCount in data.ValueCounts)
            {
                if (valueCount.Value.CompareTo(segmentation.LowestValue) < 0)
                    smaller += (ulong)valueCount.Count;
                else if (valueCount.Value.CompareTo(segmentation.LowestValue) > 0)
                    larger += (ulong)valueCount.Count;
            }

            segmentation.CountSmallerThan.AddOrUpdate(data.Attribute, smaller);
            segmentation.CountLargerThan.AddOrUpdate(data.Attribute, larger);
        }

        private IEnumerable<TableAttribute> GetAllDistinctTableAttributes(IEnumerable<IHistogram> histograms)
        {
            return histograms
                .Select(h =>
                    h.TableAttribute
                )
                .Distinct();
        }
    }
}
