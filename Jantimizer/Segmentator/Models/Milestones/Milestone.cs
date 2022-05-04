using Segmentator.Models.AbstractStorage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.Models.JsonModels;

namespace Segmentator.Models.Milestones
{
    public class Milestone : IMilestone
    {
        public IComparable LowestValue { get; set; }
        public long ElementsBeforeNextSegmentation { get; set; }
        public Dictionary<TableAttribute, IConvertible> CountSmallerThan { get; }
        public Dictionary<TableAttribute, IConvertible> CountLargerThan { get; }

        public Milestone(IComparable lowestValue, long elementsBeforeNextSegmentation)
        {
            LowestValue = lowestValue;
            ElementsBeforeNextSegmentation = elementsBeforeNextSegmentation;
            CountSmallerThan = new Dictionary<TableAttribute, IConvertible>();
            CountLargerThan = new Dictionary<TableAttribute, IConvertible>();
        }

        public ulong GetCountSmallerThanNoAlias(TableAttribute attr)
        {
            ulong retValue = 0;
            var partialKey = new TableAttribute(attr.Table.TableName, attr.Attribute);
            if (CountSmallerThan.ContainsKey(partialKey))
                retValue = Convert.ToUInt64(CountSmallerThan[partialKey]);
            return retValue;
        }

        public bool IsAnySmallerThanNoAlias(TableAttribute attr)
        {
            return GetCountSmallerThanNoAlias(attr) != 0;
        }

        public ulong GetCountLargerThanNoAlias(TableAttribute attr)
        {
            var partialKey = new TableAttribute(attr.Table.TableName, attr.Attribute);
            if (CountLargerThan.ContainsKey(partialKey))
                return Convert.ToUInt64(CountLargerThan[partialKey]);
            return 0;
        }

        public bool IsAnyLargerThanNoAlias(TableAttribute attr)
        {
            return GetCountLargerThanNoAlias(attr) != 0;
        }

        public ulong GetTotalAbstractStorageUse()
        {
            ulong returnValue = 0;

            foreach (var value in CountSmallerThan.Values)
            {
                returnValue += Convert.ToUInt64(AbstractStorageModifier.GetModifierOrOne(value.GetTypeCode()));
            }
            foreach (var value in CountLargerThan.Values)
            {
                returnValue += Convert.ToUInt64(AbstractStorageModifier.GetModifierOrOne(value.GetTypeCode()));
            }

            returnValue += Convert.ToUInt64(AbstractStorageModifier.GetModifierOrOne(Type.GetTypeCode(LowestValue.GetType())));
            returnValue += Convert.ToUInt64(AbstractStorageModifier.GetModifierOrOne(ElementsBeforeNextSegmentation.GetTypeCode()));

            return returnValue;
        }
    }
}
