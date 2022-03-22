using QueryParser.QueryParsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryParser.Exceptions
{
    public class ParserException : Exception
    {
        public List<IQueryParser> Parsers { get; }
        public string Query { get; }

        public ParserException(string message, List<IQueryParser> parsers, string query) : base(message)
        {
            Parsers = parsers;
            Query = query;
        }
    }
}
