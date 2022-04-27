using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Histograms;
using Histograms.Managers;
using Histograms.Models;
using DatabaseConnector;
using QueryOptimiser.Models;
using QueryOptimiser.Cost.Nodes;
using QueryOptimiser.Cost.EstimateCalculators.MatchFinders;
using Tools.Models.JsonModels;

namespace QueryOptimiser.Cost.EstimateCalculators
{
    public interface IEstimateCalculator
    {
        public IHistogramManager HistogramManager { get; set; }
        public JoinMatchFinder JoinMatcher { get; set; }
        public FilterMatchFinder FilterMatcher { get; set; }

        public IntermediateTable EstimateIntermediateTable(INode node, IntermediateTable intermediateTable);
    }
}
