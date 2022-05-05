using Milestoner.Models;
using Milestoner;
using Milestoner.DataGatherers;
using Milestoner.DepthCalculators;
using Milestoner.MilestoneComparers;
using Milestoner.Models.Milestones;
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
        private Dictionary<TableAttribute, List<ValueCount>> _dataCache { get; }
        private Dictionary<TableAttribute, TypeCode> _dataTypeCache { get; }

        public IMilestoneComparers Comparer { get; }

        public IDepthCalculator DepthCalculator => throw new NotImplementedException();

        public IDataGatherer DataGatherer => throw new NotImplementedException();

        public TestMilestonerManager()
        {
            Milestones = new Dictionary<TableAttribute, List<IMilestone>>();
            _dataCache = new Dictionary<TableAttribute, List<ValueCount>>();
            _dataTypeCache = new Dictionary<TableAttribute, TypeCode>();
            Comparer = new MilestoneComparer(Milestones, _dataCache, _dataTypeCache);
        }

        public Task AddMilestonesFromDB()
        {
            throw new NotImplementedException();
        }

        public void AddMilestonesFromValueCount(TableAttribute attr, List<ValueCount> sorted)
        {
            foreach (ValueCount value in sorted)
                AddOrUpdate(attr, new Milestone(value.Value, value.Value, value.Count));
        }

        public void AddMilestonesFromValueCountManual(TableAttribute attr, IComparable from, IComparable to, long count)
        {
            if (!_dataCache.ContainsKey(attr))
                _dataCache[attr] = new List<ValueCount>();
            if (!_dataTypeCache.ContainsKey(attr))
                _dataTypeCache.Add(attr, Type.GetTypeCode(from.GetType()));
            _dataCache[attr].Add(new ValueCount(from, count));
            AddOrUpdate(attr, new Milestone(from, to, count));
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

        public List<IMilestone> GetMilestonesNoAlias(TableAttribute attr)
        {
            var tempAttr = new TableAttribute(attr.Table.TableName, attr.Attribute);
            if (Milestones.ContainsKey(tempAttr))
                return Milestones[tempAttr];
            return new List<IMilestone>();
        }

        public Task<List<Func<Task>>> AddMilestonesFromDBTasks()
        {
            throw new NotImplementedException();
        }

        public List<Func<Task>> CompareMilestonesWithDBDataTasks()
        {
            throw new NotImplementedException();
        }

        public void ClearDataCache()
        {
            throw new NotImplementedException();
        }
    }
}
