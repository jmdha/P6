using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milestoner.DepthCalculators
{
    public class SquareRootDynDepth : BaseDepthCalculator, IDepthCalculator
    {
        public double YOffset { get; set; }
        public double RootMultiplier { get; set; }
        public double RootOffset { get; set; }

        public SquareRootDynDepth(bool shouldUseUniqueValues, double yOffset, double rootMultiplier, double rootOffset) : base (shouldUseUniqueValues)
        {
            YOffset = yOffset;
            RootMultiplier = rootMultiplier;
            RootOffset = rootOffset;
        }

        protected override double DepthFunction(long x)
        {
            double bucketCount = Math.Sqrt(x + RootOffset) * RootMultiplier + YOffset;

            double depth = x / bucketCount;

            return depth;
        }
    }
}
