using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QueryParser.Models;
using Histograms;
using DatabaseConnector;

namespace QueryOptimiser.Cost.Nodes
{
    internal interface INodeCost<NodeType, HistogramType, DbConnectorType>
        where NodeType : INode
        where HistogramType : IHistogram
        where DbConnectorType : IDbConnector
    {
        internal int CalculateCost(NodeType node, IHistogramManager<HistogramType, DbConnectorType> histogramManager);
    }
}
