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
        public string PlaceholderTableName { get; } = "|||";

        public bool DoesQueryMatch(string query)
        {
            if (!query.ToUpper().Contains(" JOIN "))
                return false;
            if (!query.ToUpper().Contains("SELECT "))
                return false;
            if (!query.ToUpper().Contains(" FROM "))
                return false;
            return true;
        }

        public List<INode> ParseQuery(string query)
        {
            _joinIndex = 0;

            List<INode> returnNodes = new List<INode>();

            // Remove all information in the query before the actual join statements.
            query = query.Substring(query.IndexOf(" FROM ") + 5);
            if (query[0] != '(')
                query = $"({query})";

            ParseSubQueryRec(query, returnNodes);

            StitchJoins(returnNodes);

            return returnNodes;
        }

        /// <summary>
        /// Recursively look through the Join queries, one by one reducing the query given to the next iteration.
        /// </summary>
        /// <param name="subQuery"></param>
        /// <param name="nodes"></param>
        private void ParseSubQueryRec(string subQuery, List<INode> nodes)
        {
            subQuery = TrimSubQuery(subQuery);

            if (subQuery.Contains("(") && subQuery.Contains(")"))
            {
                string innerJoinQuery = GetSubQuery(subQuery);
                ParseSubQueryRec(innerJoinQuery, nodes);
                subQuery = subQuery.Replace(innerJoinQuery, PlaceholderTableName);
            }

            nodes.Add(ParseIntoModel(subQuery));
        }

        /// <summary>
        /// Parsing of a concrete sub query into a <see cref="JoinNode"/> class
        /// </summary>
        /// <param name="subQuery"></param>
        /// <returns></returns>
        private JoinNode ParseIntoModel(string subQuery)
        {
            ComparisonType.Type type = ComparisonType.Type.None;
            string[] joinSplit = subQuery.Split(" JOIN ");
            string leftTable = "";
            string leftAttribute = "";
            string rightTable = "";
            string rightAttribute = "";
            string condition = "";
            if (joinSplit.Length > 0)
                leftTable = joinSplit[0].Trim();
            if (joinSplit.Length > 1)
            {
                string[] onSplit = joinSplit[1].Split(" ON ");

                if (onSplit.Length > 0)
                    rightTable = onSplit[0].Trim();
                if (onSplit.Length > 1)
                    condition = onSplit[1].Trim();
                if (condition.Length > 1)
                {
                    string[] conditionSplit = {"", ""};
                    var operatorTypes = (ComparisonType.Type[])Enum.GetValues(typeof(ComparisonType.Type));
                    foreach (var op in operatorTypes) {
                        if (op == ComparisonType.Type.None)
                            continue;
                        string operatorString = ComparisonType.GetOperatorString(op);
                        if (condition.Contains(operatorString)) {
                            conditionSplit = condition.Split(operatorString);
                            type = op;
                            break;
                        }
                    }
                    if (conditionSplit[0].Contains('.'))
                        leftAttribute = conditionSplit[0].Split('.')[1].Trim();
                    if (conditionSplit[1].Contains('.'))
                        rightAttribute = conditionSplit[1].Split('.')[1].Trim();
                }
            }

            var newNode = new JoinNode(_joinIndex, (string)null);
            _joinIndex++;
            return newNode;
        }

        /// <summary>
        /// Gets the subquery of the curent query, i.e. whatever is withing the next '(' and ')' characters.
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        private string GetSubQuery(string query)
        {
            if (query.Contains("(") && query.Contains(""))
                return query.Substring(query.IndexOf("("), query.LastIndexOf(")") + 1);
            return PlaceholderTableName;
        }

        /// <summary>
        /// Trims a subquery, by removing '(' and ')' so that it can be parsed later.
        /// </summary>
        /// <param name="subQuery"></param>
        /// <returns></returns>
        private string TrimSubQuery(string subQuery)
        {
            if (subQuery.Contains("(") && subQuery.Contains(""))
            {
                subQuery = subQuery.Substring(subQuery.IndexOf("(") + 1);
                subQuery = subQuery.Substring(0, subQuery.LastIndexOf(")"));
                subQuery = subQuery.Trim();
            }
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
                //if (node.LeftTable == PlaceholderTableName)
                //    node.LeftTable = GetFittingTable(node);
                //if (node.RightTable == PlaceholderTableName)
                //    node.RightTable = GetFittingTable(node);
            }
        }

        /// <summary>
        /// Finds a table that have not been used already, and puts in the placeholder.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private string GetFittingTable(JoinNode node)
        {
           /* List<string> tables = new List<string>();
            foreach(string table in node.ConditionTables)
            {
                if (table != node.LeftTable && table != node.RightTable) 
                    tables.Add(table);
            }
            if (tables.Count == 0)
                return PlaceholderTableName;*/
            return "";
        }
    }
}
