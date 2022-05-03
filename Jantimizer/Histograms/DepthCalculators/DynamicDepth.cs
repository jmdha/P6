using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Histograms.DepthCalculators
{
    public class DynamicDepth : IDepthCalculator
    {
        private bool UseUniqueValueCount { get; }
        public DynamicDepth(bool useUniqueValueCount = true) {
            UseUniqueValueCount = useUniqueValueCount;
        }

        private int SquareRootBuckets(long x)
        {
            double bucketCount = Math.Sqrt(x + 25) * 10 - 50;

            double depth = x / bucketCount;

            return (int)Math.Floor(depth);
        }

        public int GetDepth(long uniqueValueCount, long totalValueCount)
        {
            if(UseUniqueValueCount)
                return SquareRootBuckets(uniqueValueCount);
            else
                return SquareRootBuckets(totalValueCount);
        }
    }
}
