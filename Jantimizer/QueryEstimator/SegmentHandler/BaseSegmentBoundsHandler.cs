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
    public abstract class BaseSegmentBoundsHandler : ISegmentHandler
    {
        public Dictionary<TableAttribute, int> UpperBounds { get; }
        public Dictionary<TableAttribute, int> LowerBounds { get; }
        public IMilestoner Milestoner { get; }

        protected BaseSegmentBoundsHandler(Dictionary<TableAttribute, int> upperBounds, Dictionary<TableAttribute, int> lowerBounds, IMilestoner milestoner)
        {
            UpperBounds = upperBounds;
            LowerBounds = lowerBounds;
            Milestoner = milestoner;
        }

        public List<IMilestone> GetAllMilestonesForAttribute(TableAttribute attr)
        {
            return Milestoner.GetSegmentsNoAlias(attr);
        }

        public void AddOrReduceUpperBound(TableAttribute key, int bound)
        {
            if (GetValueFromDictOrAlt(key, UpperBounds, bound) < bound)
                throw new IndexOutOfRangeException("Bound upper cannot get larger!");
            AddToDictionaryIfNotThere(key, bound, UpperBounds);
        }

        public void AddOrReduceLowerBound(TableAttribute key, int bound)
        {
            if (GetValueFromDictOrAlt(key, LowerBounds, bound) > bound)
                throw new IndexOutOfRangeException("Bound lower cannot get smaller!");
            AddToDictionaryIfNotThere(key, bound, LowerBounds);
        }

        public int GetUpperBoundOrAlt(TableAttribute key, int alt)
        {
            return GetValueFromDictOrAlt(key, UpperBounds, alt);
        }

        public int GetLowerBoundOrAlt(TableAttribute key, int alt)
        {
            return GetValueFromDictOrAlt(key, LowerBounds, alt);
        }

        private void AddToDictionaryIfNotThere(TableAttribute key, int bound, Dictionary<TableAttribute, int> dict)
        {
            if (dict.ContainsKey(key))
                dict[key] = bound;
            else
                dict.Add(key, bound);
        }

        private int GetValueFromDictOrAlt(TableAttribute key, Dictionary<TableAttribute, int> dict, int alt)
        {
            if (dict.ContainsKey(key))
                return dict[key];
            return alt;
        }
    }
}
