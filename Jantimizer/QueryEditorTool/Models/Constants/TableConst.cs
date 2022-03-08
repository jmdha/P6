using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryEditorTool.Models
{
    public class TableConst : IConst
    {
        public string Name { get; set; }
        public string? Attribute { get; set; }

        public TableConst(string name, string? attribute = null)
        {
            Name = name;
            Attribute = attribute;
        }

        public override string? ToString()
        {
            if (Attribute == null)
                return Name;
            return $"{Name}.{Attribute}";
        }
    }
}
