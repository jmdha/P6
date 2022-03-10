using QueryParser.Exceptions;
using QueryParser.Models;
using QueryParser.QueryParsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryParser
{
    public class ParserManager : IParserManager
    {
        public List<IQueryParser> QueryParsers { get; internal set; }

        public ParserManager(List<IQueryParser> queryParsers)
        {
            QueryParsers = queryParsers;
        }

        public List<INode>? ParseQuery(string query)
        {
            foreach(IQueryParser parser in QueryParsers)
            {
                if (parser.DoesQueryMatch(query))
                {
                    List<INode>? resultNodes = ParseQuerySpecific(query, parser);
                    if (resultNodes != null)
                        return resultNodes;
                }
            }
            throw new ParserException("Error, no valid parser found for the query!", QueryParsers, query);
        }

        public List<INode>? ParseQuerySpecific<T>(string query, T parser) where T : IQueryParser
        {
            List<INode> nodes = parser.ParseQuery(query);
            return nodes;
        }
    }
}
