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
    }
}
