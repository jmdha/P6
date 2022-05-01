using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Histograms.DepthCalculators
{
    public class ConstantDepth
    {
        int Depth { get; }
        public ConstantDepth(int depth)
        {
            Depth = depth;
        }

        public int GetDepth(long uniqueValueCount, long totalValueCount) => Depth;
    }
}
