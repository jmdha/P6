using QueryOptimiser.Cost.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryOptimiser.Models
{
    public class OptimiserResult
    {
        public ulong EstTotalCardinality { get; set; }

        public OptimiserResult(ulong estTotalCardinality)
        {
            EstTotalCardinality = estTotalCardinality;
        }
    }
}
