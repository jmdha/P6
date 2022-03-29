using QueryParser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryParser.QueryParsers
{
    public interface IQueryParser
    {
        /// <summary>
        /// Simple method to check if this parser can actually parse the query.
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public bool DoesQueryMatch(string query);

        /// <summary>
        /// Parse the query, and return a set of <see cref="INode"/> back.
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public List<INode> ParseQuery(string query);
        public Task<List<INode>> ParseQueryAsync(string query);
    }
}
