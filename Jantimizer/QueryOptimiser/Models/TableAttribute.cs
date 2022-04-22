using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.Models.Dictionaries;

namespace QueryOptimiser.Models
{
    public class TableAttribute : DualKey
    {
        public string Name { get => Key1; set => Key1 = value; }
        public string Attribute { get => Key2; set => Key2 = value; }
        public TableAttribute(string table, string attribute) : base(table, attribute)
        {
        }
    }
}
