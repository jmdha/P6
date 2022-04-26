namespace QueryParser.Models
{
    public class TableReferenceNode : INode
    {
        public int Id { get; private set; }
        public string TableName { get; set; }
        public string Alias { get; set; }

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
                   Alias == node.Alias;
        }

        public override int GetHashCode()
        {
            int hash = HashCode.Combine(TableName, Alias);
            return hash;
        }

        public object Clone()
        {
            return new TableReferenceNode(Id, TableName, Alias);
        }
    }
}
