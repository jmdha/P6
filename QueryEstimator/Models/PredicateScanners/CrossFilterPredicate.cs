using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.Models.JsonModels;

namespace QueryEstimator.Models.PredicateScanners
{
    internal class CrossFilterPredicate : FilterPredicate
    {
        public CrossFilterPredicate(TableAttribute leftTable, IComparable constantValue, ComparisonType.Type comType) : base(leftTable, constantValue, comType)
        {
        }
    }
}
