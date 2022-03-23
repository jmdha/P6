
using DatabaseConnector;
using Histograms.Models;
using QueryParser.Models;

namespace QueryOptimiser.Cost.Nodes.EquiDepth
{
    internal interface INodeCostEquiDepth<NodeType, DbConnectorType> : INodeCost<NodeType, IHistogram, DbConnectorType>
        where NodeType : INode
        where DbConnectorType : IDbConnector
    {
    }
}
