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
        /// <summary>
        /// Gives an estimate of the cost of the operation
        /// <para>Specifically it gives the worst case cardinality estimate</para>
        /// </summary>
        /// <returns></returns>
        internal int CalculateCost(NodeType node, IHistogramManager<HistogramType, DbConnectorType> histogramManager);
    }
}
