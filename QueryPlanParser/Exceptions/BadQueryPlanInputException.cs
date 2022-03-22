using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryPlanParser.Exceptions
{
    public class BadQueryPlanInputException : Exception
    {
        public BadQueryPlanInputException(string? message) : base(message)
        {
        }
    }
}
