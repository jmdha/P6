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
        public IMilestoneComparers Comparer { get; }
        public IDepthCalculator DepthCalculator { get; }
        public IDataGatherer DataGatherer { get; }

        public BaseMilestoner(IDataGatherer dataGatherer, IDepthCalculator depthCalculator)
        {
            Milestones = new Dictionary<TableAttribute, List<IMilestone>>();
            _dataCache = new Dictionary<TableAttribute, List<ValueCount>>();
            _dataTypeCache = new Dictionary<TableAttribute, TypeCode>();
            Comparer = new MilestoneComparer(Milestones, _dataCache, _dataTypeCache);
            DepthCalculator = depthCalculator;
            DataGatherer = dataGatherer;
        }

        public List<Func<Task>> CompareMilestonesWithDBDataTasks()
        {
            // Note, this function is just for progressbars later
            return Comparer.DoMilestoneComparisonsTasks();
        }

        public async Task<List<Func<Task>>> AddMilestonesFromDBTasks()
        {
            // Note, this function is just for progressbars later
            var newList = new List<Func<Task>>();
            ClearMilestones();

            // Get all tables and attributes from the database
            var tableAttributes = new List<TableAttribute>();
            foreach (var tableName in await DataGatherer.GetTableNamesInSchema())
            {
                foreach (string attributeName in (await DataGatherer.GetAttributeNamesForTable(tableName)))
                {
                    tableAttributes.Add(new TableAttribute(tableName, attributeName));
                }
            }

            // Make a Func to start gathering from the database later
            foreach(var attr in tableAttributes)
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

        public List<IMilestone> GetMilestonesNoAlias(TableAttribute attr)
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
            Milestones.Clear();
        }

        public void ClearDataCache()
        {
            _dataCache.Clear();
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
