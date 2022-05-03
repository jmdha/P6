using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Histograms.DepthCalculators
{
    public class DynamicDepth : IDepthCalculator
    {
        public DynamicDepth() { }

        public int GetDepth(long uniqueValueCount, long totalValueCount)
        {
            long x = uniqueValueCount;
            double bucketCount = Math.Sqrt(x+25) * 10 - 50;

            double depth = x / bucketCount;

            return (int)Math.Floor(depth);
        }
    }
}
