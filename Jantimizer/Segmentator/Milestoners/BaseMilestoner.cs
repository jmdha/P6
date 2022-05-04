using Segmentator.DataGatherers;
using Segmentator.DepthCalculators;
using Segmentator.MilestoneComparers;
using Segmentator.Models;
using Segmentator.Models.Milestones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.Models.JsonModels;

namespace Segmentator.Milestoners
{
    public abstract class BaseMilestoner : IMilestoner
    {
        public Dictionary<TableAttribute, List<IMilestone>> Milestones { get; }
        public IMilestoneComparers Comparer { get; }
        public IDepthCalculator DepthCalculator { get; }
        public IDataGatherer DataGatherer { get; }

        public BaseMilestoner(IDataGatherer dataGatherer, IDepthCalculator depthCalculator)
        {
            Milestones = new Dictionary<TableAttribute, List<IMilestone>>();
            Comparer = new MilestoneComparer(Milestones);
            DepthCalculator = depthCalculator;
            DataGatherer = dataGatherer;
        }

        public async Task AddMilestonesFromDB()
        {
            ClearMilestones();
            foreach (string tableName in await DataGatherer.GetTableNamesInSchema())
            {
                foreach (string attributeName in (await DataGatherer.GetAttributeNamesForTable(tableName)))
                {
                    var newAttr = new TableAttribute(tableName, attributeName);
                    var data = await DataGatherer.GetSortedGroupsFromDb(newAttr);
                    AddMilestonesFromValueCount(newAttr, data);
                }
            }

            Comparer.DoMilestoneComparisons();
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
            Milestones.Clear();
        }

        internal void AddOrUpdate(TableAttribute attr, IMilestone milestone)
        {
            if (Milestones.ContainsKey(attr))
                Milestones[attr].Add(milestone);
            else
                Milestones.Add(attr, new List<IMilestone>() { milestone });
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
                    result += segment.GetTotalAbstractStorageUse() * (ulong)segment.ElementsBeforeNextSegmentation;
            // Converting from bit to bytes
            result = result / 8;
            return result;
        }
    }
}
