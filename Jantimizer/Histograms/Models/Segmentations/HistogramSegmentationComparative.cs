using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.Models.JsonModels;

namespace Histograms.Models
{
    public class HistogramSegmentationComparative : HistogramSegmentation, IHistogramSegmentationComparative
    {

        public Dictionary<TableAttribute, ulong> CountSmallerThan { get; } = new Dictionary<TableAttribute, ulong>();

        public Dictionary<TableAttribute, ulong> CountLargerThan { get; } = new Dictionary<TableAttribute, ulong>();

        public HistogramSegmentationComparative(IComparable lowestValue, long elementsBeforeNextSegmentation) : base(lowestValue, elementsBeforeNextSegmentation)
        {
        }

    }
}
