
using DatabaseConnector;
using Histograms;
using Histograms.Models;
using QueryOptimiser.Cost.Nodes;
using QueryOptimiser.Models;
using QueryParser.Models;

namespace QueryOptimiser.Cost.EstimateCalculators
{
    internal class EstimateCalculatorEquiDepth : BaseEstimateCalculator
    {
        public override IJoinEstimate JoinEstimator { get; set; }
        public override IFilterEstimate FilterEstimator { get; set; }
        public override MatchFinder Matcher { get; set; }

        public EstimateCalculatorEquiDepth(IHistogramManager manager) : base(manager) {
            JoinEstimator = new JoinEstimateEquiDepth();
            FilterEstimator = new FilterEstimateEquiDepth();
            Matcher = new MatchFinder(JoinEstimator, FilterEstimator);
        }
    }
}
