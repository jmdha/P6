using QueryParser.Models;

namespace QueryOptimiser.Cost.Nodes
{
    public class ValuedNode
    {
        public long Cost { get; }
        public INode Node { get; }

        public ValuedNode(long cost, INode node)
        {
            Cost = cost;
            Node = node;
        }
    }
}
