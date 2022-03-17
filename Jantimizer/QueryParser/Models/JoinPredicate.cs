namespace QueryParser.Models
{
    public partial class JoinNode
    {
        public class JoinPredicate
        {

            public TableReferenceNode LeftTable { get; internal set; }
            public string LeftAttribute { get; internal set; }
            public TableReferenceNode RightTable { get; internal set; }
            public string RightAttribute { get; internal set; }
            public string Condition { get; internal set; }
            public ComparisonType.Type ComType { get; internal set; }

            public JoinPredicate(TableReferenceNode leftTable, string leftAttribute, TableReferenceNode rightTable, string rightAttribute, string condition, ComparisonType.Type type)
            {
                LeftTable = leftTable;
                LeftAttribute = leftAttribute;
                RightTable = rightTable;
                RightAttribute = rightAttribute;
                Condition = condition;
                ComType = type;
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
