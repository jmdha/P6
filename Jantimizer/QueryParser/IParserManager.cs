using QueryParser.Models;
using QueryParser.QueryParsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.Models.JsonModels;

namespace QueryParser
{
    public interface IParserManager
    {
        /// <summary>
        /// List of active query parsers the manager can use.
        /// </summary>
        public List<IQueryParser> QueryParsers { get; set; }

        /// <summary>
        /// Parse a query, by finding the first parser that can accept the query. <see cref="IQueryParser.DoesQueryMatch(string)"/>
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public ParserResult ParseQuery(JsonQuery query, bool throwIfNotFound = true);
        public Task<ParserResult> ParseQueryAsync(JsonQuery query, bool throwIfNotFound = true);

        /// <summary>
        /// Forces a query to be parsed by a certain <see cref="IQueryParser"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="parser"></param>
        /// <returns></returns>
        public ParserResult ParseQuerySpecific<T>(JsonQuery query, T parser) where T : IQueryParser;
        public Task<ParserResult> ParseQuerySpecificAsync<T>(JsonQuery query, T parser) where T : IQueryParser;
    }
}
