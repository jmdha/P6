using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.Models.JsonModels;

namespace Histograms.Models
{
    public interface IHistogramSegmentationComparative : IHistogramSegmentation
    {
        public Dictionary<TableAttribute, ulong> CountSmallerThan { get; }
        public ulong GetCountSmallerThanNoAlias(TableAttribute attr);
        public Dictionary<TableAttribute, ulong> CountLargerThan { get; }
        public ulong GetCountLargerThanNoAlias(TableAttribute attr);
    }
}
