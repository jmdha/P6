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

            foreach (TableAttribute attribute in attributes) {
                histograms.Any(h => h.Segmentations.Any(s => s.CountLargerThan.ContainsKey(attribute)));

                foreach(var hist in histograms)
                {
                    foreach(var seg in hist.Segmentations)
                    {

                        if(seg.CountLargerThan.ContainsKey(attribute))
                        {
                            Console.WriteLine("Cheese");
                        }
                    }
                }

                DoAllComparisonsForAttribute(histograms, await DataGatherer.GetData(attribute));
            }
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

            
            if(segmentation.CountSmallerThan.ContainsKey(data.Attribute))
            {
                if (segmentation.CountSmallerThan[data.Attribute] != smaller)
                    throw new Exception("Trying to set the same attribute again, to a new value");
            }
            else
                segmentation.CountSmallerThan.Add(data.Attribute, smaller);

            if (segmentation.CountLargerThan.ContainsKey(data.Attribute))
            {
                if (segmentation.CountLargerThan[data.Attribute] != larger)
                    throw new Exception("Trying to set the same attribute again, to a new value");
            }
            else
                segmentation.CountLargerThan.Add(data.Attribute, larger);

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
