using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryParser.Exceptions
{
    public class InvalidOperatorException : InvalidPredicateException
    {
        public InvalidOperatorException(string? message, string predicate) : base(message, predicate)
        {
        }
    }
}
