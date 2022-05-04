using Milestoner.Models.Milestones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.Helpers;
using Tools.Models.JsonModels;

namespace Milestoner.MilestoneComparers
{
    public class MilestoneComparer : IMilestoneComparers
    {
        public Dictionary<TableAttribute, List<IMilestone>> Milestones { get; }

        public MilestoneComparer(Dictionary<TableAttribute, List<IMilestone>> milestones)
        {
            Milestones = milestones;
        }

        public void DoMilestoneComparisons()
        {
            foreach (var milestoneList in Milestones)
            {
                foreach (var milestone in milestoneList.Value)
                {
                    if (!IsNumericType(milestone.LowestValue.GetType()))
                        break;
                    DoMilestoneComparison(milestone);
                }
            }
        }

        private void DoMilestoneComparison(IMilestone sourceMilestone)
        {
            foreach (var milestones in Milestones)
            {
                ulong smaller = 0;
                ulong larger = 0;
                if (!IsNumericType(milestones.Value[0].LowestValue.GetType()))
                    continue;

                // We really need to find a better solution for this!
                if (Type.GetTypeCode(milestones.Value[0].LowestValue.GetType()) != Type.GetTypeCode(sourceMilestone.LowestValue.GetType()))
                    continue;

                foreach (var milestone in milestones.Value)
                {
                    var checkValue = ConvertCompareTypes(sourceMilestone.LowestValue, milestone.LowestValue);
                    if (checkValue.IsLessThan(sourceMilestone.LowestValue))
                        smaller += (ulong)milestone.ElementsBeforeNextSegmentation;
                    else if (checkValue.IsLargerThan(sourceMilestone.LowestValue))
                        larger += (ulong)milestone.ElementsBeforeNextSegmentation;
                }

                if (smaller > 0)
                {
                    if (smaller < byte.MaxValue)
                        sourceMilestone.CountSmallerThan.AddOrUpdate(milestones.Key, (byte)smaller);
                    if (smaller < ushort.MaxValue)
                        sourceMilestone.CountSmallerThan.AddOrUpdate(milestones.Key, (ushort)smaller);
                    else if (smaller < uint.MaxValue)
                        sourceMilestone.CountSmallerThan.AddOrUpdate(milestones.Key, (uint)smaller);
                    else
                        sourceMilestone.CountSmallerThan.AddOrUpdate(milestones.Key, smaller);
                }
                if (larger > 0)
                {
                    if (larger < byte.MaxValue)
                        sourceMilestone.CountLargerThan.AddOrUpdate(milestones.Key, (byte)larger);
                    if (larger < ushort.MaxValue)
                        sourceMilestone.CountLargerThan.AddOrUpdate(milestones.Key, (ushort)larger);
                    else if (larger < uint.MaxValue)
                        sourceMilestone.CountLargerThan.AddOrUpdate(milestones.Key, (uint)larger);
                    else
                        sourceMilestone.CountLargerThan.AddOrUpdate(milestones.Key, larger);
                }
            }
        }

        internal IComparable ConvertCompareTypes(IComparable segment, IComparable compare)
        {
            var compType = compare.GetType();
            var valueType = segment.GetType();
            if (compType != valueType)
                return (IComparable)Convert.ChangeType(compare, valueType);

            return compare;
        }

        private bool IsNumericType(Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return true;
                default:
                    return false;
            }
        }
    }
}
