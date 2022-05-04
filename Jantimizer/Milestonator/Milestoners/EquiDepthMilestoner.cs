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

namespace Milestoner.Milestoners
{
    public class EquiDepthMilestoner : BaseMilestoner
    {
        public EquiDepthMilestoner(IDataGatherer dataGatherer, IDepthCalculator depthCalculator) : base(dataGatherer, depthCalculator)
        {
        }

        public override void AddMilestonesFromValueCount(TableAttribute attr, List<ValueCount> sorted)
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
                            currentValueCount -= depth;
                            currentCount += depth;
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
    }
}
