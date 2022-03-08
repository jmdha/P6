using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryEditorTool.Models.Constants
{
    public class ConstVal : IConst
    {
        public INode Parent { get; set; }
        public string Value { get; set; }

        public ConstVal(INode parent, string value)
        {
            Parent = parent;
            Value = value;
        }

        public override string? ToString()
        {
            return Value;
        }
    }
}
