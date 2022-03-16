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
            /*
            List<string> tables = new List<string>();
            string query = Prefix;
            for (int i = 1; i < nodes.Count; i++)
                query += '(';
            query += nodes[0].ToString();

            if (nodes[0] is JoinNode)
            {
                JoinNode jNode = (JoinNode)nodes[0];
                tables.Add(jNode.LeftTable);
                tables.Add(jNode.RightTable);
            }
                

            for (int i = 1; i < nodes.Count; i++)
            {
                string table = "";
                if (nodes[i] is JoinNode)
                {
                    JoinNode jNode = (JoinNode)nodes[i];
                    if (!tables.Contains(jNode.LeftTable))
                        table = jNode.LeftTable;
                    else if (!tables.Contains(jNode.RightTable))
                        table = jNode.RightTable;
                    else
                        throw new Exception("Invalid join" + nodes[i].ToString());
                    tables.Add(table);
                }
                    
                query += nodes[i].GetSuffixString(table);
            }

            query += Suffix;
            return query;
            */
            return "";
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
