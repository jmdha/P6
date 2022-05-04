using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Histograms.DepthCalculators
{
    public interface IDepthCalculator
    {
        public int GetDepth(long uniqueValues, long totalValues);
    }
}
