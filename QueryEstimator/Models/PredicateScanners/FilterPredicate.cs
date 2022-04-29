using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.Models.JsonModels;

namespace QueryEstimator.Models.PredicateScanners
{
    internal class FilterPredicate : IPredicate
    {
        public TableAttribute LeftTable { get; set; }
        public IComparable ConstantValue { get; set; }
        public ComparisonType.Type ComType { get; set; }

        public FilterPredicate(TableAttribute leftTable, IComparable constantValue, ComparisonType.Type comType)
        {
            LeftTable = leftTable;
            ConstantValue = constantValue;
            ComType = comType;
        }
    }
}
