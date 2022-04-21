using QueryOptimiser.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.Exceptions;

namespace ResultsSentinel.Exceptions
{
    public class OptimiserSentinelErrorLogException : BaseErrorLogException
    {
        public OptimiserResult Result1 { get; set; }
        public OptimiserResult Result2 { get; set; }
        public OptimiserSentinelErrorLogException(Exception actualException, OptimiserResult result1, OptimiserResult result2) : base(actualException)
        {
            Result1 = result1;
            Result2 = result2;
        }

        public override string GetErrorLogMessage()
        {
            var sb = new StringBuilder();
            sb.AppendLine("Missmatch in OptimiserResults!");
            sb.AppendLine("Result 1 data:");
            sb.AppendLine(Result1.ToString());
            sb.AppendLine("Result 2 data:");
            sb.AppendLine(Result2.ToString());

            return sb.ToString();
        }
    }
}
