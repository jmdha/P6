using QueryParser.Models;
using QueryParser.QueryParsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public List<INode> ParseQuery(string query, bool throwIfNotFound = true);
        public Task<List<INode>> ParseQueryAsync(string query, bool throwIfNotFound = true);

        /// <summary>
        /// Forces a query to be parsed by a certain <see cref="IQueryParser"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="parser"></param>
        /// <returns></returns>
        public List<INode> ParseQuerySpecific<T>(string query, T parser) where T : IQueryParser;
        public Task<List<INode>> ParseQuerySpecificAsync<T>(string query, T parser) where T : IQueryParser;
    }
}
