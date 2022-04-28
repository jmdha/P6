using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Histograms.Models
{
    public class AttributeRef { }
    public interface IHistogramSegmentation
    {
        public IComparable LowestValue { get; set; }
        /// TODO: How to handle last segment?
        ///     Just add a last boundary with 1 element?


        public long ElementsBeforeNextSegmentation { get; set; }
    }
    public interface IHistogramSegmentationComparative : IHistogramSegmentation
    {
        public Dictionary<AttributeRef, ulong> CountSmallerThan { get; }
        public Dictionary<AttributeRef, ulong> CountLargerThan { get; }
    }
}
