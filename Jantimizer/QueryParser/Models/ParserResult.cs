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
        public List<INode> Nodes { get; set; }
        public string FromQuery { get; set; }

        public ParserResult(List<INode> nodes, string fromQuery)
        {
            Nodes = nodes;
            FromQuery = fromQuery;
        }

        public override string? ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine("Input Query: ");
            sb.AppendLine(FromQuery);
            sb.AppendLine("Nodes:");
            foreach (var node in Nodes)
                sb.AppendLine($"\t{node}");

            return sb.ToString();
        }

        public override bool Equals(object? obj)
        {
            return obj is ParserResult result &&
                   EqualityComparer<List<INode>>.Default.Equals(Nodes, result.Nodes) &&
                   FromQuery == result.FromQuery;
        }

        public override int GetHashCode()
        {
            int hash = 0;
            foreach (var node in Nodes)
                hash += node.GetHashCode();
            return hash + HashCode.Combine(FromQuery);
        }
    }
}
