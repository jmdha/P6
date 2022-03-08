using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryEditorTool.Models.Expersions
{
    public class BinaryExp : IExp
    {
        public INode? Parent { get; set; }
        public INode? Right { get; set; }
        public INode Value { get; set; }
        public INode? Left { get; set; }

        public BinaryExp(INode? parent, INode? right, INode value, INode? left)
        {
            Parent = parent;
            Right = right;
            Value = value;
            Left = left;
        }

        public override string? ToString()
        {
            return $"{Left}{Value}{Right}";
        }
    }
}
