using Histograms.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Histograms
{
    public class HistogramSegmenter
    {
        public List<TypeCode> AcceptedTypes { get; }
        public List<IHistogramBucket> Buckets { get; }
        public string TableName { get; }
        public string AttributeName { get; }

        List<IHistogramSegmentation> Segmentations { get; set; }


        public HistogramSegmenter(IHistogram histogram)
        {
            AcceptedTypes = histogram.AcceptedTypes;
            TableName = histogram.TableName;
            AttributeName = histogram.AttributeName;

            Segmentations = histogram.Buckets
                .Select(b =>
                    new HistogramSegmentation() {
                        LowestValue = b.ValueStart,
                        ElementsBeforeNextSegmentation = b.Count
                    }
                )
                .Cast<IHistogramSegmentation>()
                .ToList();
        }
    }
}
