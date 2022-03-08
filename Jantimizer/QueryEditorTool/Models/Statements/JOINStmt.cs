using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryEditorTool.Models
{
    public class JOINStmt : IStmt
    {
        public IConst RightTable { get; set; }
        public IConst LeftTable { get; set; }
        public IExp OnExp { get; set; }

        public JOINStmt(IConst rightTable, IConst leftTable, IExp onExp)
        {
            RightTable = rightTable;
            LeftTable = leftTable;
            OnExp = onExp;
        }

        public override string? ToString()
        {
            return $"({RightTable} JOIN {LeftTable} ON {OnExp})";
        }
    }
}
