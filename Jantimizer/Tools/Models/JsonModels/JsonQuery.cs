﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Tools.Models.JsonModels
{
    public class JsonQuery : ICloneable
    {
        public List<JoinNode> JoinNodes { get; set; }
        public string EquivalentSQLQuery { get; set; }
        public bool DoRun { get; set; }
        public string Comment { get; set; }
        public List<INode> Nodes { get {
                var newList = new List<INode>();
                newList.AddRange(JoinNodes);
                return newList;
            } 
        }
        public List<TableAttribute> TableAttributes { get {
                var newList = new List<TableAttribute>();
                foreach (var node in JoinNodes)
                    newList.AddRange(node.TableAttributes);
                return newList.Select(x => x).Distinct().ToList();
            }
        }

        public JsonQuery()
        {
            JoinNodes = new List<JoinNode>();
            EquivalentSQLQuery = "";
            DoRun = false;
            Comment = "";
        }

        public JsonQuery(List<JoinNode> joinNodes, string equivalentSQLQuery, bool doRun)
        {
            JoinNodes = joinNodes;
            EquivalentSQLQuery = equivalentSQLQuery;
            DoRun = doRun;
            Comment = "";
        }

        public JsonQuery(string fileText)
        {
            var jsonQuery = JsonSerializer.Deserialize<JsonQuery>(fileText);
            if (jsonQuery != null)
            {
                JoinNodes = jsonQuery.JoinNodes;
                EquivalentSQLQuery = jsonQuery.EquivalentSQLQuery;
                DoRun = jsonQuery.DoRun;
                Comment = jsonQuery.Comment;
            }
            else
                throw new JsonException("Could not parse the json file!");
        }

        public override string? ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"SQL Equivalent Query: {EquivalentSQLQuery}");
            sb.AppendLine($"Do run?: {DoRun}");
            sb.AppendLine($"Nodes ({Nodes.Count}):");
            foreach (var node in Nodes)
                sb.AppendLine(node.ToString());
            return sb.ToString();
        }

        public object Clone()
        {
            var newList = new List<JoinNode>();
            foreach(var node in JoinNodes)
                if (node.Clone() is JoinNode join)
                    newList.Add(join);
            return new JsonQuery(newList, EquivalentSQLQuery, DoRun);
        }

        public override int GetHashCode()
        {
            int hash = 0;
            foreach (var node in Nodes)
                hash += node.GetHashCode();
            return hash + HashCode.Combine(EquivalentSQLQuery, DoRun);
        }
    }
}
