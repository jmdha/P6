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
        internal long GetBucketEstimate(ComparisonType.Type predicate, IHistogramBucket leftBucket, IHistogramBucket rightBucket);
    }
}
