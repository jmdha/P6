using QueryParser.Models;

namespace QueryOptimiser.Cost.Nodes
{
    public class ValuedNode
    {
        public int Cost { get; }
        public INode Node { get; }

        public ValuedNode(int cost, INode node)
        {
            Cost = cost;
            Node = node;
        }
    }
}
