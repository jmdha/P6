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
            foreach(var key in CountSmallerThan.Keys)
            {
                if (key.Attribute == attr.Attribute && key.Table.TableName == attr.Table.TableName)
                    return CountSmallerThan[key];
            }
            return 0;
        }

        public bool IsAnySmallerThanNoAlias(TableAttribute attr)
        {
            return GetCountSmallerThanNoAlias(attr) != 0;
        }

        public ulong GetCountLargerThanNoAlias(TableAttribute attr)
        {
            foreach (var key in CountLargerThan.Keys)
            {
                if (key.Attribute == attr.Attribute && key.Table.TableName == attr.Table.TableName)
                    return CountLargerThan[key];
            }
            return 0;
        }

        public bool IsAnyLargerThanNoAlias(TableAttribute attr)
        {
            return GetCountLargerThanNoAlias(attr) != 0;
        }
    }
}
