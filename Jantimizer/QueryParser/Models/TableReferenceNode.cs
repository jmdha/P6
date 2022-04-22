namespace QueryParser.Models
{
    public class TableReferenceNode : INode
    {
        public int Id { get; private set; }
        public string TableName { get; set; }
        public string Alias { get; set; }
        public List<FilterNode> Filters { get; set; } = new();

        public TableReferenceNode(int id, string tableName, string alias)
        {
            Id = id;
            TableName = tableName;
            Alias = alias;
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
                   Id == node.Id &&
                   TableName == node.TableName &&
                   Alias == node.Alias &&
                   EqualityComparer<List<FilterNode>>.Default.Equals(Filters, node.Filters);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, TableName, Alias);
        }
    }
}
