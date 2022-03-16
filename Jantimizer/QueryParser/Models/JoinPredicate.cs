namespace QueryParser.Models
{
    public partial class JoinNode
    {
        public class JoinPredicate
        {
            public string LeftTable { get; internal set; }
            public string LeftAttribute { get; internal set; }
            public string RightTable { get; internal set; }
            public string RightAttribute { get; internal set; }
            public string Condition { get; internal set; }
            public ComparisonType ComType { get; internal set; }
            public List<string> ConditionTables { get; }

            public JoinPredicate(string leftTable, string leftAttribute, string rightTable, string rightAttribute, string condition, ComparisonType type)
            {
                LeftTable = leftTable;
                LeftAttribute = leftAttribute;
                RightTable = rightTable;
                RightAttribute = rightAttribute;
                Condition = condition;
                ComType = type;

                ConditionTables = GenerateConditionTables();
            }

            private List<string> GenerateConditionTables()
            {
                List<string?> tables = new List<string?>();
                SplitOverAND(Condition, tables);

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
                if (ComType == ComparisonType.More) GetTableNamesFromPredicate(s.Split(">"), tables);
                if (ComType == ComparisonType.Less) GetTableNamesFromPredicate(s.Split("<"), tables);
                if (ComType == ComparisonType.EqualOrMore) GetTableNamesFromPredicate(s.Split(">="), tables);
                if (ComType == ComparisonType.EqualOrLess) GetTableNamesFromPredicate(s.Split("<="), tables);
                if (ComType == ComparisonType.Equal) GetTableNamesFromPredicate(s.Split("="), tables);
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
}
