
using DatabaseConnector;
using Histograms;
using QueryParser.Models;

namespace QueryOptimiser.Cost.Nodes.EquiDepth
{
    internal interface INodeCostEquiDepth<NodeType, DbConnectorType> : INodeCost<NodeType, HistogramEquiDepth, DbConnectorType>
        where NodeType : INode
        where DbConnectorType : IDbConnector
    {
    }
}
