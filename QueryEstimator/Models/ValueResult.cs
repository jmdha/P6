using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.Models.JsonModels;

namespace QueryEstimator.Models
{
    internal class ValueResult : ISegmentResult
    {
        public TableAttribute TableA { get; set; }
        public TableAttribute TableB { get; set; }
        public long LeftCount { get; set; }
        public long RightCount { get; set; }

        public ValueResult(TableAttribute tableA, TableAttribute tableB, long leftCount, long rightCount)
        {
            TableA = tableA;
            TableB = tableB;
            LeftCount = leftCount;
            RightCount = rightCount;
        }

        public bool IsReferencingTableAttribute(TableAttribute attr)
        {
            if (TableA.Equals(attr))
                return true;
            if (TableB.Equals(attr))
                return true;
            return false;
        }

        public bool IsReferencingTableAttribute(List<TableAttribute> attr)
        {
            foreach (TableAttribute attr2 in attr)
                if (IsReferencingTableAttribute(attr2))
                    return true;
            return false;
        }

        public List<TableAttribute> GetTabelAttributes()
        {
            return new List<TableAttribute>() { TableA, TableB };
        }

        public long GetTotalEstimation()
        {
            return LeftCount * RightCount;
        }
    }
}
