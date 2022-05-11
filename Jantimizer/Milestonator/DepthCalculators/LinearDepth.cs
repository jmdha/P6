using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milestoner.DepthCalculators
{
    public class LinearDepth : BaseDepthCalculator, IDepthCalculator
    {
        public LinearDepth(bool shouldUseUniqueValues, double multiplier, int yOffset) : base(shouldUseUniqueValues)
        {
            Multiplier = multiplier;
            YOffset = yOffset;
        }

        public double Multiplier { get; set; }
        public int YOffset { get; set; }

        protected override double DepthFunction(long x)
        {
            return x * Multiplier + YOffset;
        }
    }
}
