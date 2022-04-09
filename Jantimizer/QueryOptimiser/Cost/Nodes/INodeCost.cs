using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QueryParser.Models;
using Histograms;
using Histograms.Models;
using DatabaseConnector;
using QueryOptimiser.Models;

namespace QueryOptimiser.Cost.Nodes
{
    internal interface INodeCost<NodeType> where NodeType : INode
    {
        /// <summary>
        /// Gives an estimate of the cost of the operation
        /// <para>Specifically it gives the worst case cardinality estimate</para>
        /// </summary>
        /// <returns></returns>
        internal IntermediateTable GetMatches(NodeType node, IHistogramManager histogramManager, IntermediateTable intermediateTable);
        internal long GetCombinedEstimate(ComparisonType.Type predicate, IHistogramBucket leftBucket, IHistogramBucket rightBucket);
    }
}
