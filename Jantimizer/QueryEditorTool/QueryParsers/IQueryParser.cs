using QueryEditorTool.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryEditorTool.QueryParsers
{
    public interface IQueryParser
    {
        public void PrintJoinIDs(INode baseTree);
        public INode? ParseQuery(string query);
    }
}
