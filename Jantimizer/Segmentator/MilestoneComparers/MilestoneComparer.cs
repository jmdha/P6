using Segmentator.Models.Milestones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.Helpers;
using Tools.Models.JsonModels;

namespace Segmentator.MilestoneComparers
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
            foreach(var milestoneList in Milestones)
            {
                foreach (var milestone in milestoneList.Value)
                {
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

                foreach (var milestone in milestones.Value)
                {
                    var checkValue = ConvertCompareTypes(sourceMilestone, milestone.LowestValue);
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

        internal IComparable ConvertCompareTypes(IMilestone segment, IComparable compare)
        {
            var compType = compare.GetType();
            var valueType = segment.LowestValue.GetType();
            if (compType != valueType)
                return (IComparable)Convert.ChangeType(compare, valueType);
            return compare;
        }
    }
}
