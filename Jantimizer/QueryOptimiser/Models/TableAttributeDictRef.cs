using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.Models.Dictionaries;
using Tools.Models.JsonModels;

namespace QueryOptimiser.Models
{
    public class TableAttributeDictRef : DualKey
    {
        public string TableName { get => Key1; set => Key1 = value; }
        public string AttributeName { get => Key2; set => Key2 = value; }
        public TableAttributeDictRef(string key1, string key2) : base(key1, key2)
        {
        }
    }
}
