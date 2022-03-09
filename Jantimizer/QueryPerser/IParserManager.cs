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
        public List<IQueryParser> QueryParsers { get; }

        public List<INode>? ParseQuery(string query);
        public List<INode>? ParseQuerySpecific<T>(string query, T parser) where T : IQueryParser;
    }
}
