using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryEditorTool.Models.Statements
{
    public class WHEREStmt : IStmt
    {
        public INode? Parent { get; set; }
        public INode? Child { get; set; }

        public WHEREStmt(INode? parent, INode? child)
        {
            Parent = parent;
            Child = child;
        }

        public override string? ToString()
        {
            return $"WHERE {Child}";
        }
    }
}
