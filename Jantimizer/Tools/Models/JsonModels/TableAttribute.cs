using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tools.Models.JsonModels
{
    public class TableAttribute
    {
        public TableReferenceNode Table { get; set; }
        public string Attribute { get; set; }

        public TableAttribute(TableReferenceNode table, string attribute)
        {
            Table = table;
            Attribute = attribute;
        }
    }
}
