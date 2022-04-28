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
        public ulong EstimatedCardinality { get; set; }

        public EstimatorResult(JsonQuery fromQuery, ulong estimatedCardinality)
        {
            FromQuery = fromQuery;
            EstimatedCardinality = estimatedCardinality;
        }

        public object Clone()
        {
            if (FromQuery.Clone() is JsonQuery query)
                return new EstimatorResult(query, EstimatedCardinality);
            throw new ArgumentNullException("Could not clone!");
        }
    }
}
