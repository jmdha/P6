using Segmentator.Models.Milestones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.Models.JsonModels;

namespace Segmentator.MilestoneComparers
{
    public interface IMilestoneComparers
    {
        public Dictionary<TableAttribute, List<IMilestone>> Milestones { get; }
        public void DoMilestoneComparisons();
    }
}
