using QueryEstimator.PredicateBounders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.Models.JsonModels;

namespace QueryEstimator.Models.BoundResults
{
    public class PredicateBoundResult<TRight> : IPredicateBoundResult<TRight>
    {
        public IPredicateBounder<TRight> Bounder { get; }
        public TableAttribute Left { get; internal set; }
        public TRight Right { get; internal set; }
        public ComparisonType.Type ComType { get; internal set; }
        public int UpperBound { get; set; }
        public int LowerBound { get; set; }

        public PredicateBoundResult(IPredicateBounder<TRight> bounder, TableAttribute left, TRight right, ComparisonType.Type comType, int upperBound, int lowerBound)
        {
            Bounder = bounder;
            Left = left;
            Right = right;
            ComType = comType;
            UpperBound = upperBound;
            LowerBound = lowerBound;
        }

        public void RecalculateBounds()
        {
            var res = Bounder.Bound(Left, Right, ComType);
            UpperBound = res.UpperBound;
            LowerBound = res.LowerBound;
        }
    }
}
