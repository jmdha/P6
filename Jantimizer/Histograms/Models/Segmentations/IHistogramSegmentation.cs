using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.Models.JsonModels;

namespace Histograms.Models
{
    public interface IHistogramSegmentation
    {
        public IComparable LowestValue { get; set; }
        /// TODO: How to handle last segment?
        ///     Just add a last boundary with 1 element?


        public long ElementsBeforeNextSegmentation { get; set; }
    }
}
