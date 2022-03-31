using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryParser.Exceptions
{
    public class InvalidPredicateException : Exception
    {
        public string Predicate { get; set; }
        public InvalidPredicateException(string? message, string predicate) : base(message)
        {
            Predicate = predicate;
        }
    }
}
