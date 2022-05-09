using QueryEstimator.PredicateBounders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.Models.JsonModels;

namespace QueryEstimator.Models.BoundResults
{
    public interface IBoundResult : ICloneable
    {
        public TableAttribute Left { get; }
        public int MaxUpperBound { get; set; }
        public int UpperBound { get; set; }
        public int MinLowerBound { get; set; }
        public int LowerBound { get; set; }
        public bool HaveChanged(Dictionary<TableAttribute, int> lowerBounds, Dictionary<TableAttribute, int> upperBounds);
    }
}
