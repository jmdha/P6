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
        public Dictionary<TableAttribute, IConvertible> CountSmallerThan { get; }
        public ulong GetCountSmallerThanNoAlias(TableAttribute attr);
        public bool IsAnySmallerThanNoAlias(TableAttribute attr);
        public Dictionary<TableAttribute, IConvertible> CountLargerThan { get; }
        public ulong GetCountLargerThanNoAlias(TableAttribute attr);
        public bool IsAnyLargerThanNoAlias(TableAttribute attr);
        public ulong GetTotalAbstractStorageUse();
    }
}
