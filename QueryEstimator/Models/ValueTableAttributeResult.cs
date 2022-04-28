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
        public TableAttribute TableB { get; set; }
        public long Count { get; set; }
        public ComparisonType.Type ComType { get; set; }

        public ValueTableAttributeResult(int tableAUpperBound, int tableALowerBound, TableAttribute tableA, TableAttribute tableB, long count, ComparisonType.Type comType)
        {
            TableAUpperBound = tableAUpperBound;
            TableALowerBound = tableALowerBound;
            TableA = tableA;
            TableB = tableB;
            Count = count;
            ComType = comType;
        }

        public long GetTotalEstimation()
        {
            return Count;
        }
    }
}
