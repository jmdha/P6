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
using Tools.Helpers;
using Tools.Models.JsonModels;

namespace Milestoner.Milestoners
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

            while (groupQueue.Count > 0)
            {
                ValueCount currentGrp = groupQueue.Dequeue();

                IComparable minValue = currentGrp.Value;
                IComparable maxValue = currentGrp.Value;
                long count = currentGrp.Count;

                while (count < depth && groupQueue.Count() > 0)
                {
                    var addValue = groupQueue.Dequeue();
                    maxValue = addValue.Value;
                    count += addValue.Count;
                }

                Milestones.AddOrUpdateList(attr, new Milestone(minValue, maxValue, count));
            }
        }
    }
}
