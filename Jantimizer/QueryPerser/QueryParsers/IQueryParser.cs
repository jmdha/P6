using QueryParser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryParser.QueryParsers
{
    public interface IQueryParser
    {
        public bool DoesQueryMatch(string query);
        public List<INode> ParseQuery(string query);
    }
}
