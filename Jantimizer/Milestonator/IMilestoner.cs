using Milestoner.DataGatherers;
using Milestoner.DepthCalculators;
using Milestoner.MilestoneComparers;
using Milestoner.Models;
using Milestoner.Models.Milestones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.Models.JsonModels;

namespace Milestoner
{
    public interface IMilestoner
    {
        public Dictionary<TableAttribute, List<IMilestone>> Milestones { get; } 
        public IMilestoneComparers Comparer { get; }
        public IDepthCalculator DepthCalculator { get; }
        public IDataGatherer DataGatherer { get; }

        public ulong GetAbstractMilestoneStorageBytes();
        public ulong GetAbstractDatabaseStorageBytes();
        public List<IMilestone> GetSegmentsNoAlias(TableAttribute attr);
        public Task AddMilestonesFromDB();
        public void AddMilestonesFromValueCount(TableAttribute attr, List<ValueCount> sorted);
        public void ClearMilestones();
    }
}
