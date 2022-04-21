using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryParser.Models
{
    public class ExplainResult
    {
        public Dictionary<string, TableReferenceNode> Tables { get; set; } = new();
        public List<JoinNode> Joins { get; set; } = new();

        public ExplainResult() { }

        public TableReferenceNode GetTableRef(string alias)
        {
            if (!Tables.ContainsKey(alias))
                throw new NullReferenceException($"No table {alias}");

            return Tables[alias];
        }
    }
}
