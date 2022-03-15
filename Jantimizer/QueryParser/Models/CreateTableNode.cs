using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryParser.Models
{
    public class CreateTableNode : INode
    {
        public int Id { get; set; }
        public string TableName { get; set; }
        public string Query { get; set; }

        public CreateTableNode(int id, string tableName, string query)
        {
            Id = id;
            TableName = tableName;
            Query = query;
        }

        public string GetSuffixString(string param)
        {
            throw new NotImplementedException();
        }
    }
}
