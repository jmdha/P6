using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryParser.Exceptions
{
    public class ParserManagerException : Exception
    {
        public ParserManagerException(string? message) : base(message)
        {
        }
    }
}
