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
    public class MinDepthMilestoner : BaseMilestoner
    {
        public MinDepthMilestoner(IDataGatherer dataGatherer, IDepthCalculator depthCalculator) : base(dataGatherer, depthCalculator)
        {
        }

        public override void AddMilestonesFromValueCount(TableAttribute attr, List<ValueCount> sorted)
        {
            var depth = DepthCalculator.GetDepth(sorted.Count(), sorted.Sum(x => x.Count));

            Queue<ValueCount> groupQueue = new Queue<ValueCount>(sorted);

            // Add all segment
            while (groupQueue.Count > 0)
            {
                ValueCount currentGrp = groupQueue.Dequeue();

                IComparable minValue = currentGrp.Value;
                long count = currentGrp.Count;

                while (count < depth && groupQueue.Count() > 0)
                {
                    count += groupQueue.Dequeue().Count;
                }

                AddOrUpdate(attr, new Milestone(minValue, count));
            }

            // Make sure the last segment is there
            if (sorted.Count > 0)
            {
                var list = GetIfThere(attr);
                if (list.Count > 0)
                {
                    if (list[list.Count - 1].ElementsBeforeNextSegmentation > 1)
                    {
                        list[list.Count - 1].ElementsBeforeNextSegmentation--;
                        AddOrUpdate(attr, new Milestone(sorted[sorted.Count - 1].Value, 1));
                    }
                }
            }
        }
    }
}
