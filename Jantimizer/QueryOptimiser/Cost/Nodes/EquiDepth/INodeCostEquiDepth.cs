using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DatabaseConnector;
using QueryOptimiser.Cost.Nodes;
using QueryParser.Models;
using Histograms;

namespace QueryOptimiser.Cost.Nodes.EquiDepth
{
    internal interface INodeCostEquiDepth<NodeType, DbConnectorType> : INodeCost<NodeType, IHistogram, DbConnectorType >
        where NodeType : INode
        where DbConnectorType : IDbConnector
    {
    }
}
