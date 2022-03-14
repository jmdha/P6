using QueryParser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryOptimiser.Cost.Nodes
{
    public class ValuedNode
    {
        internal int Cost { get; set; }
        internal INode Node { get; set; }

        public ValuedNode(int cost, INode node)
        {
            Cost = cost;
            Node = node;
        }
    }
}
