using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryEditorTool.Models.Constants
{
    public class ConstVal : IConst
    {
        public string Value { get; set; }

        public ConstVal(string value)
        {
            Value = value;
        }

        public override string? ToString()
        {
            return Value;
        }
    }
}
