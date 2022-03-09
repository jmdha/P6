using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryParser.Models
{
    public class JoinNode : INode
    {
        public int Id { get; internal set; }
        public string LeftTable { get; internal set; }
        public string RightTable { get; internal set; }
        public string JoinCondition { get; internal set; }
        public List<string> ConditionTables { get; }

        public JoinNode(int id, string leftTable, string rightTable, string joinCondition)
        {
            Id = id;
            LeftTable = leftTable.Trim();
            RightTable = rightTable.Trim();
            JoinCondition = joinCondition.Trim();
            ConditionTables = new List<string>();
            ConditionTables = GenerateConditionTables();
        }

        public override string? ToString()
        {
            return $"({LeftTable} JOIN {RightTable} ON {JoinCondition})";
        }

        private List<string> GenerateConditionTables()
        {
            List<string?> tables = new List<string?>();
            SplitOverAND(JoinCondition, tables);

            return tables
                .Where(x => x != null)
                .Select(x => x!.Trim())
                .ToList();
        }

        private void SplitOverAND(string s, List<string?> tables)
        {
            string[] andSplit = s.Split(" AND ");
            if (andSplit.Length > 0)
            {
                foreach (string and in andSplit)
                    SplitOverOR(and, tables);
            }
            else
                SplitOverOR(s, tables);
        }

        private void SplitOverOR(string s, List<string?> tables)
        {
            string[] orSplit = s.Split(" OR ");
            if (orSplit.Length > 0)
            {
                foreach (string or in orSplit)
                    SplitOverPredicates(or, tables);
            }
            else
                SplitOverPredicates(s, tables);
        }
        private void SplitOverPredicates(string s, List<string?> tables)
        {
            if (s.Contains(">")) GetTableNamesFromPredicate(s.Split(">"), tables);
            if (s.Contains("<")) GetTableNamesFromPredicate(s.Split("<"), tables);
            if (s.Contains(">=")) GetTableNamesFromPredicate(s.Split(">="), tables);
            if (s.Contains("<=")) GetTableNamesFromPredicate(s.Split("<="), tables);
            if (s.Contains("=")) GetTableNamesFromPredicate(s.Split("="), tables);
        }

        private void GetTableNamesFromPredicate(string[] s, List<string?> tables)
        {
            if (s.Length > 0)
                tables.Add(GetTableNameFromAttributeName(s[0]));
            if (s.Length > 1)
                tables.Add(GetTableNameFromAttributeName(s[1]));
        }

        private string? GetTableNameFromAttributeName(string s)
        {
            if (s.Contains("."))
                return s.Split(".")[0].Trim();
            return null;
        }
    }
}
