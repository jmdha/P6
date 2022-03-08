using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryEditorTool.Models
{
    internal class SELECTStmt : IValue, IExp
    {
        public INode? Parent { get; set; }
        public INode? Right { get; set; }
        public INode? Left { get; set; }
        public INode? Value { get; set; }

        public SELECTStmt(INode? parent, INode? right, INode? left, INode? value)
        {
            Parent = parent;
            Right = right;
            Left = left;
            Value = value;
        }

        public override string? ToString()
        {
            return $"SELECT {Value} FROM {Right}";
        }
    }
}
