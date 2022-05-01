using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Histograms.DepthCalculators
{
    public class DynamicDepth
    {
        public DynamicDepth() { }

        public int GetDepth(long uniqueValueCount, long totalValueCount)
        {
            var bucketsCount = uniqueValueCount / Math.Log10((uniqueValueCount + 11) / 11);
            return (int)Math.Ceiling(uniqueValueCount / bucketsCount);
        }
    }
}
