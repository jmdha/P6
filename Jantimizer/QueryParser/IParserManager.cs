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
        /// Parse a query, by finding the first parser that:
        /// <list type="number">
        ///     <item>Can accept the query. <see cref="IQueryParser.DoesQueryMatch(string)"/></item>
        ///     <item>Does not return a null <see cref="List"/> of <see cref="INode"/>s</item>
        /// </list>
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public List<INode> ParseQuery(string query, bool throwIfNotFound = true);

        /// <summary>
        /// Forces a query to be parsed by a certain <see cref="IQueryParser"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="parser"></param>
        /// <returns></returns>
        public List<INode>? ParseQuerySpecific<T>(string query, T parser) where T : IQueryParser;
    }
}
