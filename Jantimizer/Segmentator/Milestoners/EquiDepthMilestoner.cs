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
    public class EquiDepthMilestoner : IMilestoner
    {
        public Dictionary<TableAttribute, List<IMilestone>> Milestones { get; }
        public IMilestoneComparers Comparer { get; }
        public IDepthCalculator DepthCalculator { get; }
        public IDataGatherer DataGatherer { get; }

        public EquiDepthMilestoner(IDataGatherer dataGatherer, IDepthCalculator depthCalculator)
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

        public void AddMilestonesFromValueCount(TableAttribute attr, List<ValueCount> sorted)
        {
            long totalValues = 0;
            foreach (var value in sorted)
                totalValues += value.Count;
            var depth = DepthCalculator.GetDepth(sorted.Count, totalValues);

            IComparable? currentStart = null;
            long currentCount = 0;
            foreach (var value in sorted)
            {
                long currentValueCount = value.Count;

                while (currentValueCount > 0)
                {
                    if (currentStart == null)
                        currentStart = value.Value;
                    if (currentCount + currentValueCount < depth)
                    {
                        currentCount += currentValueCount;
                        break;
                    }
                    else
                    {
                        if (currentCount + currentValueCount > depth)
                        {
                            currentValueCount -= depth - currentCount;
                            currentCount += currentValueCount;
                        }
                        else
                        {
                            currentCount += currentValueCount;
                            currentValueCount = 0;
                        }
                        if (currentStart != null)
                            AddOrUpdate(attr, new Milestone(currentStart, currentCount));
                        currentStart = null;
                        currentCount = 0;
                    }
                }
            }
            if (currentCount == 0 && currentStart != null)
                AddOrUpdate(attr, new Milestone(currentStart, currentCount));
            if (currentCount > 0 && currentStart != null)
            {
                AddOrUpdate(attr, new Milestone(currentStart, currentCount - 1));
                var addFinal = sorted[sorted.Count - 1];
                AddOrUpdate(attr, new Milestone(addFinal.Value, 1));
            }
        }

        public void ClearMilestones()
        {
            Milestones.Clear();
        }

        private void AddOrUpdate(TableAttribute attr, IMilestone milestone)
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
