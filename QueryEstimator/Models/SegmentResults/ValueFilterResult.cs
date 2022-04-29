using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.Models.JsonModels;

namespace QueryEstimator.Models
{
    internal class ValueFilterResult : ISegmentResult
    {
        public int TableAUpperBound { get; set; }
        public int TableALowerBound { get; set; }
        public TableAttribute TableA { get; set; }
        public IComparable ConstantValue { get; set; }
        public ComparisonType.Type ComType { get; set; }

        public ValueFilterResult(int tableAUpperBound, int tableALowerBound, TableAttribute tableA, IComparable constantValue, ComparisonType.Type comType)
        {
            TableAUpperBound = tableAUpperBound;
            TableALowerBound = tableALowerBound;
            TableA = tableA;
            ConstantValue = constantValue;
            ComType = comType;
        }

        public long GetTotalEstimation()
        {
            return 1;
        }

        public override int GetHashCode()
        {
            return TableA.GetHashCode() + HashCode.Combine(TableALowerBound, TableAUpperBound, ConstantValue, ComparisonType.GetOperatorString(ComType));
        }
    }
}
