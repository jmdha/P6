using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Histograms
{
    public interface IHistogramBucket
    {
        public int ValueStart { get; set; }
        public int Count { get; set; }
    }
}
