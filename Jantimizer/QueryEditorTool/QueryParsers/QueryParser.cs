using QueryEditorTool.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace QueryEditorTool.QueryParsers
{
    public class QueryParser : IQueryParser
    {
        private int _movableIndex = 0;

        public List<JoinNode> ParseQuery(string query)
        {
            List<JoinNode> returnNodes = new List<JoinNode>();

            return returnNodes;
        }
    }
}
