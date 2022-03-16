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

        public ParserResult() { }
    }
}
