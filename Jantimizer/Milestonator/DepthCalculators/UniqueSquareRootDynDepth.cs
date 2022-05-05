using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milestoner.DepthCalculators
{
    public class UniqueSquareRootDynDepth : IDepthCalculator
    {
        public double YOffset { get; set; }
        public double RootMultiplier { get; set; }
        public double RootOffset { get; set; }

        public UniqueSquareRootDynDepth(double yOffset, double rootMultiplier, double rootOffset)
        {
            YOffset = yOffset;
            RootMultiplier = rootMultiplier;
            RootOffset = rootOffset;
        }

        private int SquareRootBuckets(long x)
        {
            double bucketCount = Math.Sqrt(x + RootOffset) * RootMultiplier + YOffset;

            double depth = x / bucketCount;

            return (int)Math.Floor(depth);
        }

        public int GetDepth(long uniqueValueCount, long totalValueCount)
        {
            return SquareRootBuckets(uniqueValueCount);
        }
    }
}
