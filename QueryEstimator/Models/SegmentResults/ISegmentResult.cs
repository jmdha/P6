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
        public long GetTotalEstimation();
        public bool DoesContainTableAttribute(TableAttribute attr);
    }
}
