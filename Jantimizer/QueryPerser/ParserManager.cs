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
            return null;
        }

        public List<INode>? ParseQuerySpecific<T>(string query, T parser) where T : IQueryParser
        {
            try
            {
                List<INode> nodes = parser.ParseQuery(query);
                return nodes;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error, the parser [{nameof(parser)}] could not process the query:");
                Console.WriteLine($"\t {query}");
                Console.WriteLine("Exception message:");
                Console.WriteLine(ex.Message);
            }
            return null;
        }
    }
}
