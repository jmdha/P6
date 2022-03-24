using QueryParser.Models;

namespace QueryOptimiser.Models
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
