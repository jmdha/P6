using QueryOptimiser.Models;
using System.Text;
using Tools.Models.Dictionaries;

namespace ResultsSentinel
{
    public class OptimiserResultSentinel : BaseResultSentinel<OptimiserResult>
    {
        public static OptimiserResultSentinel? Instance;

        public OptimiserResultSentinel() : base()
        {
            Instance = this;
            Criticality = SentinelCriticality.High;
        }

        public override string GetErrorDescription(OptimiserResult value1, OptimiserResult value2)
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