namespace Tools.Models.JsonModels
{
    public class TableReferenceNode : INode
    {
        public string TableName { get; set; }
        public string Alias { get; set; }

        public TableReferenceNode(string tableName, string alias)
        {
            TableName = tableName;
            Alias = alias;
        }
        public TableReferenceNode(string tableName)
        {
            TableName = tableName;
            Alias = tableName;
        }

        public override string ToString()
        {
            if (Alias == TableName)
                return TableName;

            return $"{TableName} AS {Alias}";
        }

        public override bool Equals(object? obj)
        {
            return obj is TableReferenceNode node &&
                   TableName == node.TableName &&
                   Alias == node.Alias;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(TableName, Alias);
        }

        public object Clone()
        {
            return new TableReferenceNode(TableName, Alias);
        }
    }
}
