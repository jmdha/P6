using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Histograms
{
    public interface IHistogram
    {
        public IHistogramBucket[] Buckets { get; set; }
    }
}
