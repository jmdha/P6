namespace QueryParser.Models
{
    public class TableReferenceNode : INode
    {
        public int Id { get; private set; }
        public string TableName { get; set; }
        public string Alias { get; set; }
        public List<FilterNode> Filters { get; set; }

        public TableReferenceNode(int id, string tableName, string alias)
        {
            Id = id;
            TableName = tableName;
            Alias = alias;
            Filters = new List<FilterNode>();
        }

        public TableReferenceNode(int id, string tableName, string alias, List<FilterNode> filters)
        {
            Id = id;
            TableName = tableName;
            Alias = alias;
            Filters = filters;
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

        public object Clone()
        {
            var newFilter = new List<FilterNode>();
            foreach (var node in Filters)
                if (node.Clone() is FilterNode clone)
                    newFilter.Add(clone);
            return new TableReferenceNode(Id, TableName, Alias);
        }
    }
}
