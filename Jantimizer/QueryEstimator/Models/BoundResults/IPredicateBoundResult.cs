using QueryEstimator.PredicateBounders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.Models.JsonModels;

namespace QueryEstimator.Models.BoundResults
{
    public interface IPredicateBoundResult<TRight> : ICloneable
    {
        public IPredicateBounder<TRight> Bounder { get; }
        public TableAttribute Left { get; }
        public TRight Right { get; }
        public ComparisonType.Type ComType { get; }
        public int UpperBound { get; set; }
        public int LowerBound { get; set; }
        public bool HaveChanged(Dictionary<TableAttribute, int> lowerBounds, Dictionary<TableAttribute, int> upperBounds);

        public void RecalculateBounds();
    }
}
