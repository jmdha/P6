using Milestoner.DataGatherers;
using Milestoner.Models;
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
        private Dictionary<TableAttribute, List<ValueCount>> _dataCache { get; }
        private Dictionary<TableAttribute, TypeCode> _dataTypeCache { get; }

        public MilestoneComparer(Dictionary<TableAttribute, List<IMilestone>> milestones, Dictionary<TableAttribute, List<ValueCount>> dataCache, Dictionary<TableAttribute, TypeCode> dataTypeCache)
        {
            Milestones = milestones;
            _dataCache = dataCache;
            _dataTypeCache = dataTypeCache;
        }

        public List<Func<Task>> DoMilestoneComparisonsTasks()
        {
            var newList = new List<Func<Task>>();
            foreach (var tableAttribute in Milestones.Keys)
            {
                Func<Task> runFunc = () => Task.Run(() => DoMilestoneComparison(tableAttribute));
                newList.Add(runFunc);
            }
            return newList;
        }

        public void DoMilestoneComparisons()
        {
            foreach (var tableAttribute in Milestones.Keys)
            {
                DoMilestoneComparison(tableAttribute);
            }
        }

        public void DoMilestoneComparison(TableAttribute tableAttribute)
        {
            if (_dataCache.ContainsKey(tableAttribute) && _dataTypeCache.ContainsKey(tableAttribute))
            {
                if (!IsNumericType(_dataTypeCache[tableAttribute]))
                    return;

                foreach (var milestone in Milestones[tableAttribute])
                {
                    foreach (var otherTableAttribute in Milestones.Keys)
                    {
                        if (_dataTypeCache[otherTableAttribute] != _dataTypeCache[tableAttribute])
                            continue;
                        DoMilestoneComparison(milestone, otherTableAttribute, _dataCache[otherTableAttribute]);
                    }
                }
            }
        }

        private void DoMilestoneComparison(IMilestone sourceMilestone, TableAttribute compareKey, List<ValueCount> compareValues)
        {
            ulong smaller = 0;
            ulong larger = 0;

            foreach (var value in compareValues)
            {
                if (value.Value.IsLessThan(sourceMilestone.HighestValue))
                    smaller += (ulong)value.Count;
                else if (value.Value.IsLargerThan(sourceMilestone.LowestValue))
                    larger += (ulong)value.Count;
            }

            if (smaller > 0)
            {
                if (smaller < byte.MaxValue)
                    sourceMilestone.CountSmallerThan.AddOrUpdate(compareKey, (byte)smaller);
                if (smaller < ushort.MaxValue)
                    sourceMilestone.CountSmallerThan.AddOrUpdate(compareKey, (ushort)smaller);
                else if (smaller < uint.MaxValue)
                    sourceMilestone.CountSmallerThan.AddOrUpdate(compareKey, (uint)smaller);
                else
                    sourceMilestone.CountSmallerThan.AddOrUpdate(compareKey, smaller);
            }
            if (larger > 0)
            {
                if (larger < byte.MaxValue)
                    sourceMilestone.CountLargerThan.AddOrUpdate(compareKey, (byte)larger);
                if (larger < ushort.MaxValue)
                    sourceMilestone.CountLargerThan.AddOrUpdate(compareKey, (ushort)larger);
                else if (larger < uint.MaxValue)
                    sourceMilestone.CountLargerThan.AddOrUpdate(compareKey, (uint)larger);
                else
                    sourceMilestone.CountLargerThan.AddOrUpdate(compareKey, larger);
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

        private bool IsNumericType(TypeCode typeCode)
        {
            switch (typeCode)
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
