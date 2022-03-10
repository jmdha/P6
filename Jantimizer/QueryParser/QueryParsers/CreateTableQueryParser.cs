using QueryParser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryParser.QueryParsers
{
    public class CreateTableQueryParser : IQueryParser
    {
        private int _createTableId = 0;

        public bool DoesQueryMatch(string query)
        {
            query = query.Trim().ToUpper();
            if (query.StartsWith("CREATE ") && query.Contains(" TABLE "))
                return true;
            return false;
        }

        public List<INode> ParseQuery(string query)
        {
            string[] querySplit = query.ToUpper().Split(" TABLE ");
            if (querySplit.Length > 1)
            {
                string rightSide = querySplit[1];
                string tableName = rightSide.Split(" ")[0].ToLower();
                var returnNode = new CreateTableNode(_createTableId, tableName, rightSide);
                _createTableId++;
                return new List<INode>() { returnNode };
            }
            return new List<INode>();
        }
    }
}
