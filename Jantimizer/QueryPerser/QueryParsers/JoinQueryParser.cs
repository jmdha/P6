using QueryParser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace QueryParser.QueryParsers
{
    public class JoinQueryParser : IQueryParser
    {
        private int _joinIndex = 0;
        public string PlaceholderTableName { get; } = "...";

        public bool DoesQueryMatch(string query)
        {
            if (query.ToUpper().Contains("JOIN"))
                return true;
            return false;
        }

        public List<INode> ParseQuery(string query)
        {
            List<INode> returnNodes = new List<INode>();

            query = query.Substring(query.IndexOf("FROM") + 4);
            if (query[0] != '(')
                query = $"({query})";

            ParseSubQuery(query, returnNodes);

            StitchJoins(returnNodes);

            return returnNodes;
        }

        private void ParseSubQuery(string subQuery, List<INode> nodes)
        {
            subQuery = TrimSubQuery(subQuery);

            if (subQuery.Contains("(") && subQuery.Contains(")"))
            {
                ParseSubQuery(subQuery.Substring(subQuery.IndexOf("("), subQuery.LastIndexOf(")") + 1), nodes);
                subQuery = subQuery.Replace(subQuery.Substring(subQuery.IndexOf("("), subQuery.LastIndexOf(")") + 1), PlaceholderTableName);
            }

            string leftTable = subQuery.Split("JOIN")[0];
            string rightSide = subQuery.Split("JOIN")[1].Trim();

            string rightTable = rightSide.Split("ON")[0];
            string condition = rightSide.Split("ON")[1];

            var newNode = new JoinNode(_joinIndex, leftTable, rightTable, condition);
            _joinIndex++;
            nodes.Add(newNode);
        }

        /// <summary>
        /// Trims a subquery, by removing '(' and ')' so that it can be parsed later.
        /// </summary>
        /// <param name="subQuery"></param>
        /// <returns></returns>
        private string TrimSubQuery(string subQuery)
        {
            subQuery = subQuery.Substring(subQuery.IndexOf("(") + 1);
            subQuery = subQuery.Substring(0, subQuery.LastIndexOf(")"));
            subQuery = subQuery.Trim();
            return subQuery;
        }

        /// <summary>
        /// To stitch join subqueries back together
        /// 
        /// <code>
        ///     Example
        ///     "... JOIN B ON B.Value1 > A.Value2"
        ///     becomes
        ///     "A JOIN B ON B.Value1 > A.Value2"
        /// </code>
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private void StitchJoins(List<INode> nodes)
        {
            foreach(JoinNode node in nodes)
            {
                if (node.LeftTable == PlaceholderTableName)
                    node.LeftTable = GetFittingTable(node);
                if (node.RightTable == PlaceholderTableName)
                    node.RightTable = GetFittingTable(node);
            }
        }

        /// <summary>
        /// Finds a table that have not been used already, and puts in the placeholder.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private string GetFittingTable(JoinNode node)
        {
            List<string> tables = new List<string>();
            foreach(string table in node.ConditionTables)
            {
                if (table != node.LeftTable && table != node.RightTable) 
                    tables.Add(table);
            }
            if (tables.Count == 0)
                return PlaceholderTableName;
            return tables[0];
        }
    }
}
