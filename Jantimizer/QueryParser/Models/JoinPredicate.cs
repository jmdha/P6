namespace QueryParser.Models
{
    public class JoinPredicate : ICloneable
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

        public override int GetHashCode()
        {
            return LeftTable.GetHashCode() + RightTable.GetHashCode() + HashCode.Combine(LeftAttribute, RightAttribute, Condition, ComparisonType.GetOperatorString(ComType));
        }

        public object Clone()
        {
            if (LeftTable.Clone() is TableReferenceNode left)
                if (RightTable.Clone() is TableReferenceNode right)
                    return new JoinPredicate(left, LeftAttribute, right, RightAttribute, Condition, ComType);
            throw new InvalidOperationException();
        }
    }
}
