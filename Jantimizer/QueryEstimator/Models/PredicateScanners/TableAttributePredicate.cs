using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.Models.JsonModels;

namespace QueryEstimator.Models.PredicateScanners
{
    public class TableAttributePredicate : IPredicate
    {
        public TableAttribute LeftTable { get; set; }
        public TableAttribute RightTable { get; set; }
        public ComparisonType.Type ComType { get; set; }
        public bool TreatAsFilter { get; set; }

        public TableAttributePredicate(TableAttribute leftTable, TableAttribute rightTable, ComparisonType.Type comType, bool treatAsFilter)
        {
            LeftTable = leftTable;
            RightTable = rightTable;
            ComType = comType;
            TreatAsFilter = treatAsFilter;
        }
    }
}
