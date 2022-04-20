using QueryParser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryParser.QueryParsers
{
    public class ParserResult
    {
        public Dictionary<string, TableReferenceNode> Tables { get; set; } = new();
        public List<JoinNode> Joins { get; set; } = new();
        public List<FilterNode> Filters { get; set; } = new();

        public ParserResult() { }

        public TableReferenceNode GetTableRef(string alias)
        {
            if (!Tables.ContainsKey(alias))
                throw new NullReferenceException($"No table {alias}");

            return Tables[alias];
        }

        public List<INode> GetNodes()
        {
            List<INode> nodes = new List<INode>();

            nodes.AddRange(Joins);
            nodes.AddRange(Filters);

            return nodes;
        }
    }
}
