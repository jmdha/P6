using Segmentator.MilestoneComparers;
using Segmentator.Models.Milestones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.Models.JsonModels;

namespace Segmentator.Milestoners
{
    public class EquiDepthMilestoner : IMilestoner
    {
        public Dictionary<TableAttribute, List<IMilestone>> Milestones { get; }
        public IMilestoneComparers Comparer { get; }

        public EquiDepthMilestoner()
        {
            Milestones = new Dictionary<TableAttribute, List<IMilestone>>();
            Comparer = new MilestoneComparer(Milestones);
        }

        public Task<List<Task>> AddMilestonesFromDB()
        {
            
        }

        private void GenerateHistogramFromSorted(List<IComparable> sorted)
        {
            var depth = DepthCalculator.GetDepth(sorted.GroupBy(x => x).Count(), sorted.Count);
            for (int bStart = 0; bStart < sorted.Count; bStart += depth)
            {
                IComparable startValue = sorted[bStart];
                IComparable endValue = sorted[bStart];
                int countValue = 1;

                for (int bIter = bStart + 1; bIter < bStart + depth && bIter < sorted.Count; bIter++)
                {
                    countValue++;
                    endValue = sorted[bIter];
                }
                Buckets.Add(new HistogramBucket(startValue, endValue, countValue));
            }
        }

        public void ClearMilestones()
        {
            
        }
    }
}
