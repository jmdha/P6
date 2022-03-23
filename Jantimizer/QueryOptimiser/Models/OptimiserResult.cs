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
        public List<ValuedNode> EstCardinalities { get; set; }

        public OptimiserResult(ulong estTotalCardinality, List<ValuedNode> estCardinalities)
        {
            EstTotalCardinality = estTotalCardinality;
            EstCardinalities = estCardinalities;
        }
    }
}
