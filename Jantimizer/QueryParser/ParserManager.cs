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

        public ParserResult ParseQuery(string query, bool throwIfNotFound = true)
        {
            try
            {
                foreach (IQueryParser parser in QueryParsers)
                {
                    if (parser.DoesQueryMatch(query))
                        return ParseQuerySpecific(query, parser);
                }
            }
            catch (Exception ex)
            {
                throw new ParserErrorLogException(ex, QueryParsers, query);
            }
            if (throwIfNotFound)
                throw new ParserErrorLogException(new ParserManagerException("Error, no valid parser found for the query!"), QueryParsers, query);
            return new ParserResult(query);
        }

        public async Task<ParserResult> ParseQueryAsync(string query, bool throwIfNotFound = true)
        {
            try
            {
                foreach (IQueryParser parser in QueryParsers)
                {
                    if (await parser.DoesQueryMatchAsync(query))
                        return await ParseQuerySpecificAsync(query, parser);
                }
            }
            catch (Exception ex)
            {
                throw new ParserErrorLogException(ex, QueryParsers, query);
            }
            if (throwIfNotFound)
                throw new ParserErrorLogException(new ParserManagerException("Error, no valid parser found for the query!"), QueryParsers, query);
            return new ParserResult(query);
        }

        public ParserResult ParseQuerySpecific<T>(string query, T parser) where T : IQueryParser
        {
            return parser.ParseQuery(query);
        }

        public async Task<ParserResult> ParseQuerySpecificAsync<T>(string query, T parser) where T : IQueryParser
        {
            return await parser.ParseQueryAsync(query);
        }
    }
}
