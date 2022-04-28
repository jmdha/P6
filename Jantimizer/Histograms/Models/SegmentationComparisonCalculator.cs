using Histograms.DataGatherers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        private List<TableAttribute> TableAttributes { get; set; }
        private List<TableAttribute> UsedAttributes { get; } = new List<TableAttribute> { };

        public async Task DoHistogramComparisons(IEnumerable<IHistogram> histograms)
        {
            IEnumerable<TableAttribute> attributes = GetAllDistinctTableAttributes(histograms);
            TableAttributes = attributes.ToList();
            foreach (TableAttribute attribute in attributes)
                DoAllComparisonsForAttribute(histograms, await DataGatherer.GetData(attribute));
        }



        private void DoAllComparisonsForAttribute(IEnumerable<IHistogram> histograms, AttributeData data)
        {
            UsedAttributes.Add(data.Attribute);
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

            int cheese;
            try
            {
                cheese = 1;
                segmentation.CountSmallerThan.Add(data.Attribute, smaller);
                cheese = 2;
                segmentation.CountLargerThan.Add(data.Attribute, larger);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine("Cheese");
                throw ex;
            }

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
