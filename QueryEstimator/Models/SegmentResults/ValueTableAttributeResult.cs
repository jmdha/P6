using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.Models.JsonModels;

namespace QueryEstimator.Models
{
    internal class ValueTableAttributeResult : ISegmentResult
    {
        public int TableAUpperBound { get; set; }
        public int TableALowerBound { get; set; }
        public TableAttribute TableA { get; set; }
        public int TableBUpperBound { get; set; }
        public int TableBLowerBound { get; set; }
        public TableAttribute TableB { get; set; }
        public long Count { get; set; }
        public ComparisonType.Type ComType { get; set; }

        public ValueTableAttributeResult(int tableAUpperBound, int tableALowerBound, TableAttribute tableA, int tableBUpperBound, int tableBLowerBound, TableAttribute tableB, long count, ComparisonType.Type comType)
        {
            TableAUpperBound = tableAUpperBound;
            TableALowerBound = tableALowerBound;
            TableA = tableA;
            TableBUpperBound = tableBUpperBound;
            TableBLowerBound = tableBLowerBound;
            TableB = tableB;
            Count = count;
            ComType = comType;
        }

        public long GetTotalEstimation()
        {
            return Count;
        }

        public override int GetHashCode()
        {
            return TableA.GetHashCode() + TableB.GetHashCode() + HashCode.Combine(TableALowerBound, TableAUpperBound, TableBLowerBound, TableBUpperBound, Count, ComparisonType.GetOperatorString(ComType));
        }

        public bool DoesContainTableAttribute(TableAttribute attr)
        {
            if (TableA.Equals(attr))
                return true;
            if (TableB.Equals(attr))
                return true;
            return false;
        }
    }
}
