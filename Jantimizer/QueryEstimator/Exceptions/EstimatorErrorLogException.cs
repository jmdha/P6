using QueryEstimator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.Exceptions;
using Tools.Models.JsonModels;

namespace QueryEstimator.Exceptions
{
    public class EstimatorErrorLogException : BaseErrorLogException
    {
        public JsonQuery FromQuery { get; set; }

        public EstimatorErrorLogException(Exception actualException, JsonQuery fromQuery) : base(actualException)
        {
            FromQuery = fromQuery;
        }

        public override string GetErrorLogMessage()
        {
            return $"{FromQuery}";
        }
    }
}
