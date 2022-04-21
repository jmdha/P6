namespace QueryParser.Models
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

        public override bool Equals(object? obj)
        {
            return obj is JoinPredicate predicate &&
                   EqualityComparer<TableReferenceNode>.Default.Equals(LeftTable, predicate.LeftTable) &&
                   LeftAttribute == predicate.LeftAttribute &&
                   EqualityComparer<TableReferenceNode>.Default.Equals(RightTable, predicate.RightTable) &&
                   RightAttribute == predicate.RightAttribute &&
                   Condition == predicate.Condition &&
                   ComType == predicate.ComType;
        }

        public override int GetHashCode()
        {
            return LeftTable.GetHashCode() + RightTable.GetHashCode() + HashCode.Combine(LeftAttribute, RightAttribute, Condition, ComType);
        }
    }
}
