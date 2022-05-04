using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.Models.JsonModels;

namespace QueryEstimator.Models
{
    public class ValueTableAttributeResult : ISegmentResult
    {
        public TableAttribute TableA { get; set; }
        public TableAttribute TableB { get; set; }
        public long Count { get; set; }
        public ComparisonType.Type ComType { get; set; }

        public ValueTableAttributeResult(TableAttribute tableA, TableAttribute tableB, long count, ComparisonType.Type comType)
        {
            TableA = tableA;
            TableB = tableB;
            Count = count;
            ComType = comType;
        }

        public long GetTotalEstimation()
        {
            return Count;
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
