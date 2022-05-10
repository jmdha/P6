using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milestoner.DepthCalculators
{
    public abstract class BaseDepthCalculator : IDepthCalculator
    {
        public bool ShouldUseUniqueValues { get; set; }

        public BaseDepthCalculator(bool shouldUseUniqueValues)
        {
            ShouldUseUniqueValues = shouldUseUniqueValues;
        }

        protected abstract double DepthFunction(long x);
        public int GetDepth(long uniqueValues, long totalValues)
        {
            long num = ShouldUseUniqueValues ? uniqueValues : totalValues;

            // Floored depth, with a minimum value of 1
            return (int)Math.Floor(Math.Max(1, DepthFunction(num)));
        }
    }
}
