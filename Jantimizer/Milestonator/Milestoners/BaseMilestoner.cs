using Milestoner.DataGatherers;
using Milestoner.DepthCalculators;
using Milestoner.MilestoneComparers;
using Milestoner.Models;
using Milestoner.Models.AbstractStorage;
using Milestoner.Models.Milestones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.Models.JsonModels;

namespace Milestoner.Milestoners
{
    public abstract class BaseMilestoner : IMilestoner
    {
        public Dictionary<TableAttribute, List<IMilestone>> Milestones { get; }
        private Dictionary<TableAttribute, List<ValueCount>> _dataCache { get; }
        private Dictionary<TableAttribute, TypeCode> _dataTypeCache { get; }
        private List<TableAttribute> _tableAttributes;
        public IMilestoneComparers Comparer { get; }
        public IDepthCalculator DepthCalculator { get; }
        public IDataGatherer DataGatherer { get; }

        public BaseMilestoner(IDataGatherer dataGatherer, IDepthCalculator depthCalculator)
        {
            Milestones = new Dictionary<TableAttribute, List<IMilestone>>();
            _dataCache = new Dictionary<TableAttribute, List<ValueCount>>();
            _dataTypeCache = new Dictionary<TableAttribute, TypeCode>();
            _tableAttributes = new List<TableAttribute>();
            Comparer = new MilestoneComparer(Milestones, _dataCache, _dataTypeCache);
            DepthCalculator = depthCalculator;
            DataGatherer = dataGatherer;
        }

        public List<Func<Task>> CompareMilestonesWithDBDataTasks()
        {
            return Comparer.DoMilestoneComparisonsTasks();
        }

        public async Task<List<Func<Task>>> AddMilestonesFromDBTasks()
        {
            var newList = new List<Func<Task>>();
            ClearMilestones();

            foreach(var tableName in await DataGatherer.GetTableNamesInSchema())
            {
                foreach (string attributeName in (await DataGatherer.GetAttributeNamesForTable(tableName)))
                {
                    _tableAttributes.Add(new TableAttribute(tableName, attributeName));
                }
            }

            foreach(var attr in _tableAttributes)
            {
                Func<Task> runFunc = async () => {
                    var data = await DataGatherer.GetSortedGroupsFromDb(attr);
                    var dataTypeCode = await DataGatherer.GetTypeCodeFromDb(attr);
                    _dataCache.Add(attr, data);
                    _dataTypeCache.Add(attr, dataTypeCode);
                    AddMilestonesFromValueCount(attr, data);
                };
                newList.Add(runFunc);
            }

            return newList;
        }

        public List<IMilestone> GetSegmentsNoAlias(TableAttribute attr)
        {
            var tempAttr = new TableAttribute(attr.Table.TableName, attr.Attribute);
            if (Milestones.ContainsKey(tempAttr))
                return Milestones[tempAttr];
            return new List<IMilestone>();
        }

        public abstract void AddMilestonesFromValueCount(TableAttribute attr, List<ValueCount> sorted);

        public void ClearMilestones()
        {
            _dataCache.Clear();
            _dataTypeCache.Clear();
            _tableAttributes.Clear();
            Milestones.Clear();
        }

        public void ClearDataCache()
        {
            _dataCache.Clear();
        }

        internal void AddOrUpdate(TableAttribute attr, IMilestone milestone)
        {
            if (Milestones.ContainsKey(attr))
                Milestones[attr].Add(milestone);
            else
                Milestones.Add(attr, new List<IMilestone>() { milestone });
        }

        internal List<IMilestone> GetIfThere(TableAttribute attr)
        {
            if (Milestones.ContainsKey(attr))
                return Milestones[attr];
            else
                return new List<IMilestone>();
        }

        public ulong GetAbstractMilestoneStorageBytes()
        {
            ulong result = 0;
            // Get all bytes from all segments.
            foreach (var segments in Milestones.Values)
                foreach (var segment in segments)
                    result += segment.GetTotalAbstractStorageUse();
            // Converting from bit to bytes
            result = result / 8;
            return result;
        }

        public ulong GetAbstractDatabaseStorageBytes()
        {
            ulong result = 0;
            // Get all bytes from all segments.
            foreach (var segments in Milestones.Values)
                foreach (var segment in segments)
                    result += Convert.ToUInt64(AbstractStorageModifier.GetModifierOrOne(Type.GetTypeCode(segment.LowestValue.GetType()))) * (ulong)segment.ElementsBeforeNextSegmentation;
            // Converting from bit to bytes
            result = result / 8;
            return result;
        }
    }
}
