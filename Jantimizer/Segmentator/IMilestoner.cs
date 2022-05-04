using Segmentator.MilestoneComparers;
using Segmentator.Models.Milestones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.Models.JsonModels;

namespace Segmentator
{
    public interface IMilestoner
    {
        public Dictionary<TableAttribute, List<IMilestone>> Milestones { get; } 
        public IMilestoneComparers Comparer { get; }

        public Task<List<Task>> AddMilestonesFromDB();
        public void ClearMilestones();
    }
}
