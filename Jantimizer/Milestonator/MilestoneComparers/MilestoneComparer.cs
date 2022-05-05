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

        public MilestoneComparer(Dictionary<TableAttribute, List<IMilestone>> milestones, Dictionary<TableAttribute, List<ValueCount>> dataCache)
        {
            Milestones = milestones;
            _dataCache = dataCache;
        }

        public void DoMilestoneComparisons()
        {
            foreach (var compareKey in Milestones.Keys)
            {
                if (_dataCache.ContainsKey(compareKey) && _dataCache[compareKey].Count > 0)
                {
                    var data = _dataCache[compareKey];
                    foreach (var milestoneList in Milestones)
                    {
                        foreach (var sourceMilestone in milestoneList.Value)
                        {
                            // We really need to find a better solution for this!
                            if (Type.GetTypeCode(sourceMilestone.LowestValue.GetType()) != Type.GetTypeCode(data[0].Value.GetType()))
                                continue;

                            DoMilestoneComparison(sourceMilestone, compareKey, data);
                        }
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
                var checkValue = ConvertCompareTypes(sourceMilestone.LowestValue, value.Value);
                if (checkValue.IsLessThan(sourceMilestone.LowestValue))
                    smaller += (ulong)value.Count;
                else if (checkValue.IsLargerThan(sourceMilestone.LowestValue))
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
    }
}
