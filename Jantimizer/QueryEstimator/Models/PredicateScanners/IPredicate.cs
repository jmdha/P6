using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.Models.JsonModels;

namespace QueryEstimator.Models.PredicateScanners
{
    public interface IPredicate
    {
        public TableAttribute LeftTable { get; set; }
        public ComparisonType.Type ComType { get; set; }
    }
}
