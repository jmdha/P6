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
            return $"{TableName} AS {Alias}";
        }

        public string GetSuffixString(string param)
        {
            throw new NotImplementedException();
        }
    }
}
