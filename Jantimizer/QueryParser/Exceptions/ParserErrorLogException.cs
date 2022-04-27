using QueryParser.QueryParsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.Exceptions;
using Tools.Models.JsonModels;

namespace QueryParser.Exceptions
{
    public class ParserErrorLogException : BaseErrorLogException
    {
        public List<IQueryParser> Parsers { get; }
        public JsonQuery Query { get; }

        public ParserErrorLogException(Exception actualException, List<IQueryParser> parsers, JsonQuery query) : base(actualException)
        {
            Parsers = parsers;
            Query = query;
        }

        public override string GetErrorLogMessage()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Query: {Query}");
            sb.AppendLine($"Query Parsers:");
            foreach(var parser in Parsers)
                sb.AppendLine($"\t{parser.GetType().Name}");
            return sb.ToString();
        }
    }
}
