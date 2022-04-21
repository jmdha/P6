
using DatabaseConnector;
using Histograms;
using Histograms.Models;
using QueryOptimiser.Cost.Nodes;
using QueryOptimiser.Models;
using QueryParser.Models;

namespace QueryOptimiser.Cost.EstimateCalculators
{
    internal class EstimateCalculatorVariance : BaseEstimateCalculator
    {
        public override IJoinEstimate JoinEstimator { get; set; }
        public override IFilterEstimate FilterEstimator { get; set; }
        public override MatchFinder Matcher { get; set; }

        public EstimateCalculatorVariance(IHistogramManager manager) : base(manager) {
            JoinEstimator = new JoinEstimateEquiDepthVariance();
            FilterEstimator = new FilterEstimateEquiDepthVariance();
            Matcher = new MatchFinder(JoinEstimator, FilterEstimator);
        }
    }
}
