using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.Models.JsonModels;

namespace QueryEstimator.Models
{
    internal class SegmentResult : ISegmentResult
    {
        public ISegmentResult Left { get; set; }
        public ISegmentResult Right { get; set; }

        public SegmentResult(ISegmentResult left, ISegmentResult right)
        {
            Left = left;
            Right = right;
        }

        public bool IsReferencingTableAttribute(TableAttribute attr)
        {
            if (Left.IsReferencingTableAttribute(attr))
                return true;
            if (Right.IsReferencingTableAttribute(attr))
                return true;
            return false;
        }

        public long GetTotalEstimation()
        {
            return Left.GetTotalEstimation() * Right.GetTotalEstimation();
        }
    }
}
