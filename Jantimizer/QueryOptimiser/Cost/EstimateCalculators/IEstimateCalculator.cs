using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QueryParser.Models;
using Histograms;
using Histograms.Managers;
using Histograms.Models;
using DatabaseConnector;
using QueryOptimiser.Models;
using QueryOptimiser.Cost.Nodes;

namespace QueryOptimiser.Cost.EstimateCalculators
{
    public interface IEstimateCalculator
    {
        public IHistogramManager HistogramManager { get; }
        public INodeCost<JoinNode> NodeCostCalculator { get; }

        public IntermediateTable EstimateIntermediateTable(INode node, IntermediateTable intermediateTable);
    }
}
