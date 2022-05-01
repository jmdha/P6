using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.Models.JsonModels;

namespace Histograms.Models
{
    public class HistogramSegmentationComparative : HistogramSegmentation, IHistogramSegmentationComparative
    {

        public Dictionary<TableAttribute, ulong> CountSmallerThan { get; } = new Dictionary<TableAttribute, ulong>();
        public Dictionary<TableAttribute, ulong> CountLargerThan { get; } = new Dictionary<TableAttribute, ulong>();

        public HistogramSegmentationComparative(IComparable lowestValue, long elementsBeforeNextSegmentation) : base(lowestValue, elementsBeforeNextSegmentation)
        {
        }

        public ulong GetCountSmallerThanNoAlias(TableAttribute attr)
        {
            var partialKey = new TableAttribute(attr.Table.TableName, attr.Attribute);
            if (CountSmallerThan.ContainsKey(partialKey))
                return CountSmallerThan[partialKey];
            return 0;
        }

        public ulong GetCountLargerThanNoAlias(TableAttribute attr)
        {
            var partialKey = new TableAttribute(attr.Table.TableName, attr.Attribute);
            if (CountLargerThan.ContainsKey(partialKey))
                return CountLargerThan[partialKey];
            return 0;
        }
    }
}
