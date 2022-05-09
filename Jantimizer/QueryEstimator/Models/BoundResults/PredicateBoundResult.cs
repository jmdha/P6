using QueryEstimator.PredicateBounders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.Models.JsonModels;

namespace QueryEstimator.Models.BoundResults
{
    public class PredicateBoundResult<TRight> : IReboundableResult<TRight>
    {
        public IPredicateBounder<TRight> Bounder { get; }
        public TableAttribute Left { get; internal set; }
        public TRight Right { get; internal set; }
        public ComparisonType.Type ComType { get; internal set; }
        public int MaxUpperBound { get; set; }
        public int UpperBound { get; set; }
        public int MinLowerBound { get; set; }
        public int LowerBound { get; set; }

        public PredicateBoundResult(IPredicateBounder<TRight> bounder, TableAttribute left, TRight right, ComparisonType.Type comType, int maxUpperBound, int upperBound, int minLowerBound, int lowerBound)
        {
            Bounder = bounder;
            Left = left;
            Right = right;
            ComType = comType;
            MaxUpperBound = maxUpperBound;
            UpperBound = upperBound;
            MinLowerBound = minLowerBound;
            LowerBound = lowerBound;
        }

        public void RecalculateBounds()
        {
            var res = Bounder.Bound(Left, Right, ComType);
            UpperBound = res.UpperBound;
            LowerBound = res.LowerBound;
        }

        public bool HaveChanged(Dictionary<TableAttribute, int> lowerBounds, Dictionary<TableAttribute, int> upperBounds)
        {
            return LowerBound != lowerBounds[Left] || UpperBound != upperBounds[Left];
        }

        public object Clone()
        {
            return new PredicateBoundResult<TRight>(Bounder, Left, Right, ComType, MaxUpperBound, UpperBound, MinLowerBound, LowerBound);
        }
    }
}
