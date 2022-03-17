using QueryOptimiser.Cost.Nodes;
using QueryParser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryGenerator.QueryGenerators
{
    public class PostgresGenerator : IQueryGenerator
    {
        
        static string Prefix = "SELECT * FROM ";
        static string Suffix = ";";

        public string GenerateQuery(List<INode> nodes)
        {
            
            List<TableReferenceNode> tables = new List<TableReferenceNode>();
            string query = Prefix;
            for (int i = 1; i < nodes.Count; i++)
                query += '(';
            query += nodes[0].ToString();

            if (nodes[0] is JoinNode)
            {
                JoinNode jNode = (JoinNode)nodes[0];
                tables.AddRange(jNode.Relation.GetJoinTables());
            }
                

            for (int i = 1; i < nodes.Count; i++)
            {
                TableReferenceNode table = new TableReferenceNode(-1, "...", "...");
                if (nodes[i] is JoinNode)
                {
                    JoinNode jNode = (JoinNode)nodes[i];

                    bool leftContains = false;
                    List<TableReferenceNode> leftJoinTables = jNode.Relation.GetJoinTables(true, false);
                    foreach (TableReferenceNode joinTable in leftJoinTables)
                        if (tables.Contains(joinTable))
                            leftContains = true;

                    bool rightContains = false;
                    List<TableReferenceNode> rightJoinTables = jNode.Relation.GetJoinTables(false, true);
                    foreach (TableReferenceNode joinTable in rightJoinTables)
                        if (tables.Contains(joinTable))
                            rightContains = true;

                    if (!leftContains)
                        table = jNode.Relation.LeafPredicate.LeftTable;
                    else if (!rightContains)
                        table = jNode.Relation.LeafPredicate.RightTable;
                    else
                        throw new Exception("Invalid join" + nodes[i].ToString());
                    tables.Add(table);
                }
                query += nodes[i].GetSuffixString(table.Alias);
            }

            query += Suffix;
            return query;
        }

        public string GenerateQuery(List<ValuedNode> nodes)
        {
            nodes.Sort((a, b) => a.Cost <= b.Cost ? -1 : 1);
            List<INode> sortedNodes = new List<INode>();
            for (int i = 0; i < nodes.Count; i++)
                sortedNodes.Add(nodes[i].Node);
            return GenerateQuery(sortedNodes);
        }
    }
}
