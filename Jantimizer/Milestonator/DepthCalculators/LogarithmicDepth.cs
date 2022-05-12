using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milestoner.DepthCalculators
{
    public class LogarithmicDepth : BaseDepthCalculator, IDepthCalculator
    {
        public double LogBase { get; set; }
        public double Multiplier { get; set; }
        public int XOffset { get; set; }

        public LogarithmicDepth(bool shouldUseUniqueValues, double logBase, double multiplier, int xOffset) : base (shouldUseUniqueValues)
        {
            LogBase = logBase;
            Multiplier = multiplier;
            XOffset = xOffset;
        }

        private double LogN(long x, double logBase)
        {
            return Math.Log2(x)/Math.Log2(logBase);
        }

        protected override double DepthFunction(long x)
        {
            x += XOffset;
            return x / LogN(x, LogBase) * Multiplier;
        }
    }
}
