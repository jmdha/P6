using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.Models.JsonModels;

namespace QueryEstimator.Models
{
    public interface ISegmentResult
    {
        public bool IsReferencingTableAttribute(TableAttribute attr);
        public bool IsReferencingTableAttribute(List<TableAttribute> attr);
        public List<TableAttribute> GetTabelAttributes();
        public long GetTotalEstimation();
    }
}
