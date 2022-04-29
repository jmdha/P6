using QueryEstimator.Models;
using System.Text;
using Tools.Models.Dictionaries;

namespace ResultsSentinel
{
    public class EstimatorResultSentinel : BaseResultSentinel<EstimatorResult>
    {
        public static EstimatorResultSentinel? Instance;

        public EstimatorResultSentinel() : base()
        {
            Instance = this;
            Criticality = SentinelCriticality.High;
        }

        public override string GetErrorDescription(EstimatorResult value1, EstimatorResult value2)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Missmatch in OptimiserResults!");
            sb.AppendLine("Expected data:");
            sb.AppendLine(value1.ToString());
            sb.AppendLine("Actual data:");
            sb.AppendLine(value2.ToString());

            return sb.ToString();
        }
    }
}