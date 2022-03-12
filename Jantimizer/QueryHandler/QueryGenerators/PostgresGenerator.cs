using QueryParser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryOptimiser.QueryGenerators
{
    public class PostgresGenerator : IQueryGenerator
    {
        static string Prefix = "SELECT * FROM ";
        static string Suffix = ";";

        public string GenerateQuery(List<INode> nodes)
        {
            string query = Prefix;
            for (int i = 1; i < nodes.Count; i++)
                query += '(';
            query += nodes[0].ToString();

            for (int i = 1; i < nodes.Count; i++)
            {
                query += nodes[i].GetSuffixString();
            }

            query += Suffix;
            return query;
        }

        public string GenerateQuery(List<Tuple<INode, int>> nodes)
        {
            nodes.Sort((a, b) => a.Item2 <= b.Item2 ? -1 : 1);
            List<INode> sortedNodes = new List<INode>();
            for (int i = 0; i < nodes.Count; i++)
                sortedNodes.Add(nodes[i].Item1);
            return GenerateQuery(sortedNodes);
        }
    }
}


// (a join b on a = b) join c on b = c <=> (b join c on b = c) join a on a = b
// ((a join b on a = b) join c on b = c) join c on a = c <=> ((a join c on a = c) join b on a = b) join c on b = c