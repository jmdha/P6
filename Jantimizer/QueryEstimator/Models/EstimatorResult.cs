using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.Models.JsonModels;

namespace QueryEstimator.Models
{
    public class EstimatorResult : ICloneable
    {
        public JsonQuery FromQuery { get; set; }
        public int DidXSweeps { get; set; }
        public ulong EstimatedCardinality { get; set; }

        public EstimatorResult(JsonQuery fromQuery, ulong estimatedCardinality, int didXSweeps)
        {
            FromQuery = fromQuery;
            EstimatedCardinality = estimatedCardinality;
            DidXSweeps = didXSweeps;
        }

        public object Clone()
        {
            if (FromQuery.Clone() is JsonQuery query)
                return new EstimatorResult(query, EstimatedCardinality, DidXSweeps);
            throw new ArgumentNullException("Could not clone!");
        }

        public override int GetHashCode()
        {
            return FromQuery.GetHashCode() + HashCode.Combine(EstimatedCardinality, DidXSweeps);
        }

        public override string? ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine("Json Query was:");
            sb.AppendLine(FromQuery.ToString());
            sb.AppendLine("Estimated Cardinality was:");
            sb.AppendLine($"{EstimatedCardinality}");
            sb.AppendLine("After x sweeps:");
            sb.AppendLine($"{DidXSweeps}");

            return sb.ToString();
        }
    }
}
