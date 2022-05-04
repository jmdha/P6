using Histograms;
using Histograms.Models;
using Segmentator;
using Segmentator.Models.Milestones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.Models.JsonModels;

namespace QueryEstimator.SegmentHandler
{
    public interface ISegmentHandler
    {
        public Dictionary<TableAttribute, int> UpperBounds { get; }
        public Dictionary<TableAttribute, int> LowerBounds { get; }
        public IMilestoner Milestoner { get; }

        public List<IMilestone> GetAllMilestonesForAttribute(TableAttribute attr);

        public void AddOrReduceUpperBound(TableAttribute key, int bound);
        public void AddOrReduceLowerBound(TableAttribute key, int bound);

        public int GetUpperBoundOrAlt(TableAttribute key, int alt);
        public int GetLowerBoundOrAlt(TableAttribute key, int alt);
    }
}
