using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryEditorTool.Models
{
    internal class SELECTStmt : IStmt
    {
        public IConst SelectValues { get; set; }
        public IStmt Statement { get; set; }

        public SELECTStmt(IConst selectValues, IStmt statement)
        {
            SelectValues = selectValues;
            Statement = statement;
        }

        public override string? ToString()
        {
            return $"SELECT {SelectValues} FROM {Statement}";
        }
    }
}
