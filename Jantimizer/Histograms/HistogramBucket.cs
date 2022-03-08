using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Histograms
{
    public class HistogramBucket : IHistogramBucket
    {
        public int ValueStart { get; set; }
        public int ValueEnd { get; set; }
        public int Count { get; set; }
    }
}
