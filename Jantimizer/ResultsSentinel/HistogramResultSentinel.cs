using Histograms;
using Histograms.Models;
using Histograms.Models.Histograms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResultsSentinel
{
    public class HistogramResultSentinel : BaseResultSentinel<HistogramSet>
    {
        public static HistogramResultSentinel? Instance;

        public HistogramResultSentinel() : base()
        {
            Instance = this;
            Criticality = SentinelCriticality.High;
        }

        public override string GetErrorDescription(HistogramSet value1, HistogramSet value2)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Missmatch in Used Histogram!");
            sb.AppendLine("Expected data:");
            sb.AppendLine(value1.ToString());
            sb.AppendLine("Actual data:");
            sb.AppendLine(value2.ToString());

            return sb.ToString();
        }
    }
}
