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
        public List<IQueryParser> QueryParsers { get; set; }

        public ParserManager(List<IQueryParser> queryParsers)
        {
            QueryParsers = queryParsers;
        }

        public List<INode> ParseQuery(string query, bool throwIfNotFound = true)
        {
            foreach (IQueryParser parser in QueryParsers)
            {
                if (parser.DoesQueryMatch(query))
                    return ParseQuerySpecific(query, parser);
            }
            if (throwIfNotFound)
                throw new ParserException("Error, no valid parser found for the query!", QueryParsers, query);
            return new List<INode>();
        }

        public async Task<List<INode>> ParseQueryAsync(string query, bool throwIfNotFound = true)
        {
            foreach (IQueryParser parser in QueryParsers)
            {
                if (parser.DoesQueryMatch(query))
                    return await ParseQuerySpecificAsync(query, parser);
            }
            if (throwIfNotFound)
                throw new ParserException("Error, no valid parser found for the query!", QueryParsers, query);
            return new List<INode>();
        }

        public List<INode> ParseQuerySpecific<T>(string query, T parser) where T : IQueryParser
        {
            List<INode> nodes = parser.ParseQuery(query);
            return nodes;
        }

        public async Task<List<INode>> ParseQuerySpecificAsync<T>(string query, T parser) where T : IQueryParser
        {
            List<INode> nodes = await parser.ParseQueryAsync(query);
            return nodes;
        }
    }
}
