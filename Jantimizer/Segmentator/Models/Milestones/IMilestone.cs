using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.Models.JsonModels;

namespace Segmentator.Models.Milestones
{
    public interface IMilestone
    {
        public IComparable LowestValue { get; set; }
        public long ElementsBeforeNextSegmentation { get; set; }

        public Dictionary<TableAttribute, IConvertible> CountSmallerThan { get; }
        public Dictionary<TableAttribute, IConvertible> CountLargerThan { get; }

        public ulong GetCountSmallerThanNoAlias(TableAttribute attr);
        public bool IsAnySmallerThanNoAlias(TableAttribute attr);
        public ulong GetCountLargerThanNoAlias(TableAttribute attr);
        public bool IsAnyLargerThanNoAlias(TableAttribute attr);
        public ulong GetTotalAbstractStorageUse();
    }
}
