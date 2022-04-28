using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.Models.JsonModels;

namespace QueryEstimator.Models
{
    internal class TableAttributePredicate
    {
        public TableAttribute LeftTable { get; set; }
        public TableAttribute RightTable { get; set; }
        public ComparisonType.Type ComType { get; set; }

        public TableAttributePredicate(TableAttribute leftTable, TableAttribute rightTable, ComparisonType.Type comType)
        {
            LeftTable = leftTable;
            RightTable = rightTable;
            ComType = comType;
        }
    }
}
