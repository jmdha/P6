using Segmentator.Models;
using Segmentator;
using Segmentator.DataGatherers;
using Segmentator.DepthCalculators;
using Segmentator.MilestoneComparers;
using Segmentator.Models.Milestones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.Models.JsonModels;

namespace QueryEstimatorTests.Stubs
{
    internal class TestMilestonerManager : IMilestoner
    {
        public Dictionary<TableAttribute, List<IMilestone>> Milestones { get; }

        public IMilestoneComparers Comparer { get; }

        public IDepthCalculator DepthCalculator => throw new NotImplementedException();

        public IDataGatherer DataGatherer => throw new NotImplementedException();

        public TestMilestonerManager()
        {
            Milestones = new Dictionary<TableAttribute, List<IMilestone>>();
            Comparer = new MilestoneComparer(Milestones);
        }

        public Task AddMilestonesFromDB()
        {
            throw new NotImplementedException();
        }

        public void AddMilestonesFromValueCount(TableAttribute attr, List<ValueCount> sorted)
        {
            foreach (ValueCount value in sorted)
                AddOrUpdate(attr, new Milestone(value.Value, value.Count));
        }

        public void ClearMilestones()
        {
            throw new NotImplementedException();
        }

        public ulong GetAbstractDatabaseStorageBytes()
        {
            throw new NotImplementedException();
        }

        public ulong GetAbstractMilestoneStorageBytes()
        {
            throw new NotImplementedException();
        }

        private void AddOrUpdate(TableAttribute attr, IMilestone milestone)
        {
            if (Milestones.ContainsKey(attr))
                Milestones[attr].Add(milestone);
            else
                Milestones.Add(attr, new List<IMilestone>() { milestone });
        }

        public List<IMilestone> GetSegmentsNoAlias(TableAttribute attr)
        {
            var tempAttr = new TableAttribute(attr.Table.TableName, attr.Attribute);
            if (Milestones.ContainsKey(tempAttr))
                return Milestones[tempAttr];
            return new List<IMilestone>();
        }
    }
}
