
using DatabaseConnector;
using Histograms;
using Histograms.Models;
using QueryOptimiser.Cost.EstimateCalculators.MatchFinders;
using QueryOptimiser.Cost.Nodes;
using QueryOptimiser.Models;
using QueryParser.Models;

namespace QueryOptimiser.Cost.EstimateCalculators
{
    internal class EstimateCalculatorEquiDepth : BaseEstimateCalculator
    {
        public override JoinMatchFinder JoinMatcher { get; set; }
        public override FilterMatchFinder FilterMatcher { get; set; }

        internal EstimateCalculatorEquiDepth(IHistogramManager manager) : base(manager) {
            JoinMatcher = new JoinMatchFinder(new JoinEstimateEquiDepth());
            FilterMatcher = new FilterMatchFinder(new FilterEstimateEquiDepth());
        }
    }
}
