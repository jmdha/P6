using QueryParser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryParser.QueryParsers
{
    public class ParserResult : ICloneable
    {
        public Dictionary<string, TableReferenceNode> Tables { get; set; }
        public List<JoinNode> Joins { get; set; }
        public List<FilterNode> Filters { get; set; }
        public List<INode> Nodes { get {
                var newList = new List<INode>();
                newList.AddRange(Joins);
                newList.AddRange(Filters);
                return newList;
            } 
        }
        public string FromQuery { get; set; }

        public ParserResult()
        {
            Tables = new Dictionary<string, TableReferenceNode>();
            Joins = new List<JoinNode>();
            Filters = new List<FilterNode>();
            FromQuery = "";
        }

        public ParserResult(string fromQuery) : this()
        {
            FromQuery = fromQuery;
        }

        public ParserResult(List<JoinNode> joinNodes, List<FilterNode> filterNodes, Dictionary<string, TableReferenceNode> tables, string fromQuery)
        {
            Tables = tables;
            Joins = joinNodes;
            Filters = filterNodes;
            FromQuery = fromQuery;
        }

        public TableReferenceNode GetTableRef(string alias)
        {
            if (!Tables.ContainsKey(alias))
                throw new NullReferenceException($"No table {alias}");

            return Tables[alias];
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
                   EqualityComparer<Dictionary<string, TableReferenceNode>>.Default.Equals(Tables, result.Tables) &&
                   EqualityComparer<List<JoinNode>>.Default.Equals(Joins, result.Joins) &&
                   EqualityComparer<List<FilterNode>>.Default.Equals(Filters, result.Filters) &&
                   EqualityComparer<List<INode>>.Default.Equals(Nodes, result.Nodes) &&
                   FromQuery == result.FromQuery;
        }

        public override int GetHashCode()
        {
            int hash = 0;
            foreach (var node in Nodes)
                hash += node.GetHashCode();
            foreach (var table in Tables.Values)
                hash += table.GetHashCode();
            return hash + HashCode.Combine(FromQuery);
        }

        public object Clone()
        {
            var newJoinList = new List<JoinNode>();
            foreach(var node in Joins)
                if (node.Clone() is JoinNode clone)
                    newJoinList.Add(clone);
            var newFilterList = new List<FilterNode>();
            foreach (var node in Filters)
                if (node.Clone() is FilterNode clone)
                    newFilterList.Add(clone);
            var newDict = new Dictionary<string, TableReferenceNode>();
            foreach (var key in Tables.Keys)
                if (Tables[key].Clone() is TableReferenceNode clone)
                    newDict.Add(key, clone);
            return new ParserResult(newJoinList, newFilterList, newDict, FromQuery);
        }
    }
}
