using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Histograms.Models
{
    public class HistogramSegmentation : IHistogramSegmentation
    {
        public HistogramSegmentation(IComparable lowestValue, long elementsBeforeNextSegmentation)
        {
            LowestValue = lowestValue;
            ElementsBeforeNextSegmentation = elementsBeforeNextSegmentation;
        }

        public IComparable LowestValue { get; set; }
        public long ElementsBeforeNextSegmentation { get; set; }

    }
}
