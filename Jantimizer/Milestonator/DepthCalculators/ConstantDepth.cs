using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milestoner.DepthCalculators
{
    public class ConstantDepth : IDepthCalculator
    {
        int Depth { get; }
        public ConstantDepth(int depth)
        {
            Depth = depth;
        }

        public int GetDepth(long uniqueValueCount, long totalValueCount) => Depth;
    }
}
