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
            foreach (var milestone in Milestones)
            {
                ulong smaller = 0;
                ulong larger = 0;

                foreach (var milestones in milestone.Value)
                {
                    if (milestones.LowestValue.IsLessThan(sourceMilestone.LowestValue))
                        smaller += (ulong)milestones.ElementsBeforeNextSegmentation;
                    else if (milestones.LowestValue.IsLargerThan(sourceMilestone.LowestValue))
                        larger += (ulong)milestones.ElementsBeforeNextSegmentation;
                }

                if (smaller > 0)
                {
                    if (smaller < byte.MaxValue)
                        sourceMilestone.CountSmallerThan.AddOrUpdate(milestone.Key, (byte)smaller);
                    if (smaller < ushort.MaxValue)
                        sourceMilestone.CountSmallerThan.AddOrUpdate(milestone.Key, (ushort)smaller);
                    else if (smaller < uint.MaxValue)
                        sourceMilestone.CountSmallerThan.AddOrUpdate(milestone.Key, (uint)smaller);
                    else
                        sourceMilestone.CountSmallerThan.AddOrUpdate(milestone.Key, smaller);
                }
                if (larger > 0)
                {
                    if (larger < byte.MaxValue)
                        sourceMilestone.CountLargerThan.AddOrUpdate(milestone.Key, (byte)larger);
                    if (larger < ushort.MaxValue)
                        sourceMilestone.CountLargerThan.AddOrUpdate(milestone.Key, (ushort)larger);
                    else if (larger < uint.MaxValue)
                        sourceMilestone.CountLargerThan.AddOrUpdate(milestone.Key, (uint)larger);
                    else
                        sourceMilestone.CountLargerThan.AddOrUpdate(milestone.Key, larger);
                }
            }
        }
    }
}
