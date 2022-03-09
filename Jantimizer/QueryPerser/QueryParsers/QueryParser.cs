using QueryEditorTool.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace QueryEditorTool.QueryParsers
{
    public class QueryParser : IQueryParser
    {
        private int _movableIndex = 0;
        private static string _placeHolderTableName = "...";

        public List<JoinNode> ParseQuery(string query)
        {
            List<JoinNode> returnNodes = new List<JoinNode>();

            query = query.Substring(query.IndexOf("FROM") + 4);
            if (query[0] != '(')
                query = $"({query})";

            ParseSubQuery(query, returnNodes);

            StitchJoins(returnNodes);

            return returnNodes;
        }

        private void ParseSubQuery(string subQuery, List<JoinNode> nodes)
        {
            subQuery = TrimSubQuery(subQuery);

            if (subQuery.Contains("(") && subQuery.Contains(")"))
            {
                ParseSubQuery(subQuery.Substring(subQuery.IndexOf("("), subQuery.LastIndexOf(")") + 1), nodes);
                subQuery = subQuery.Replace(subQuery.Substring(subQuery.IndexOf("("), subQuery.LastIndexOf(")") + 1), _placeHolderTableName);
            }

            string leftTable = subQuery.Split("JOIN")[0];
            string rightSide = subQuery.Split("JOIN")[1].Trim();

            string rightTable = rightSide.Split("ON")[0];
            string condition = rightSide.Split("ON")[1];

            var newNode = new JoinNode(_movableIndex, leftTable, rightTable, condition);
            _movableIndex++;
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
        private void StitchJoins(List<JoinNode> nodes)
        {
            foreach(JoinNode node in nodes)
            {
                if (node.LeftTable == _placeHolderTableName)
                    node.LeftTable = GetFittingTable(node);
                if (node.RightTable == _placeHolderTableName)
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
                return _placeHolderTableName;
            return tables[0];
        }
    }
}
