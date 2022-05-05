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
            var depth = DepthCalculator.GetDepth(sorted.Count(), sorted.Sum(x => x.Count));

            IComparable? currentStart = null;
            IComparable? currentEnd = null;
            long currentCount = 0;
            foreach (var value in sorted)
            {
                long currentValueCount = value.Count;
                currentEnd = value.Value;

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
                            AddOrUpdate(attr, new Milestone(currentStart, currentEnd, currentCount));
                        currentStart = null;
                        currentCount = 0;
                    }
                }
            }
            if (currentCount == 0 && currentStart != null && currentEnd != null)
                AddOrUpdate(attr, new Milestone(currentStart, currentEnd, currentCount));
            if (currentCount > 0 && currentStart != null && currentEnd != null)
            {
                AddOrUpdate(attr, new Milestone(currentStart, currentEnd, currentCount));
            }
        }
    }
}
