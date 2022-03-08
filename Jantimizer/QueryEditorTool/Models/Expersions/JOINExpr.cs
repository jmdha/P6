using QueryEditorTool.Models.Constants;
using QueryEditorTool.Models.Expersions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryEditorTool.Models
{
    public class JOINExpr : IExp, IMovable, ICloneable
    {
        public List<ConstVal> UsedTables { get; set; } = new List<ConstVal>();
        public INode? Parent { get; set; }
        public INode? Left { get; set; }
        public INode? Right { get; set; }
        public INode? Value { get; set; }
        public int ItemIndex { get; internal set; }

        public JOINExpr(INode? parent, INode? left, INode? right, INode? value, int itemIndex)
        {
            Parent = parent;
            Left = left;
            Right = right;
            Value = value;
            ItemIndex = itemIndex;
            UsedTables = new List<ConstVal>();
        }

        public override string? ToString()
        {
            return $"({Left} JOIN {Right} ON {Value})";
        }

        public object Clone()
        {
            var returnClone = new JOINExpr(Parent, Left, Right, Value, ItemIndex);
            returnClone.GetTablesUsedInConditionRec(returnClone.UsedTables, returnClone.Value);
            return returnClone;
        }

        public void GetTablesUsedInConditionRec(List<ConstVal> tables, INode parent)
        {
            if (parent is ConstVal tableValue)
            {
                if (tableValue.Value.Contains('.'))
                    tables.Add(new ConstVal(parent, tableValue.Value.Split('.')[0]));
            }
            if (parent is IExp exp)
            {
                GetTablesUsedInConditionRec(tables, exp.Right);
                GetTablesUsedInConditionRec(tables, exp.Left);
                GetTablesUsedInConditionRec(tables, exp.Value);
            }
        }
    }
}
