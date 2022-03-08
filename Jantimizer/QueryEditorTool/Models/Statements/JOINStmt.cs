using QueryEditorTool.Models.Constants;
using QueryEditorTool.Models.Expersions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryEditorTool.Models
{
    public class JOINStmt : IValue, IExp, IMovable, ICloneable
    {
        public List<ConstVal> UsedTables { get; set; } = new List<ConstVal>();
        public INode? Parent { get; set; }
        public INode? Left { get; set; }
        public INode? Right { get; set; }
        public INode? Value { get; set; }
        public int ItemIndex { get; internal set; }
        public bool IsInnerJoin { get; }

        public JOINStmt(INode? parent, INode? left, INode? right, INode? value, int itemIndex)
        {
            Parent = parent;
            Left = left;
            Right = right;
            Value = value;
            ItemIndex = itemIndex;

            if (Right is ConstVal && Left is ConstVal)
                IsInnerJoin = true;
            else
                IsInnerJoin = false;
        }

        public override string? ToString()
        {
            return $"({Left} JOIN {Right} ON {Value})";
        }

        public object Clone()
        {
            return new JOINStmt(Parent, Left, Right, Value, ItemIndex);
        }
    }
}
