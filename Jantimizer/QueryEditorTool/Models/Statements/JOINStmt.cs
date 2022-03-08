using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryEditorTool.Models
{
    public class JOINStmt : IStmt, IMovable
    {
        public INode? RightTable { get; set; }
        public INode? LeftTable { get; set; }
        public IExp? OnExp { get; set; }
        public int ItemIndex { get; internal set; }

        public JOINStmt(INode? rightTable, INode? leftTable, IExp? onExp, int itemIndex)
        {
            RightTable = rightTable;
            LeftTable = leftTable;
            OnExp = onExp;
            ItemIndex = itemIndex;
        }

        public override string? ToString()
        {
            return $"({RightTable} JOIN {LeftTable} ON {OnExp})";
        }
    }
}
