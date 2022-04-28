using QueryEstimator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.Models.JsonModels;

namespace QueryEstimator.Mergers
{
    public class SegmentListMerger : IMerger<
        Dictionary<TableAttribute, int>, 
        List<ISegmentResult>, 
        Dictionary<TableAttribute, List<ISegmentResult>>>
    {
        public Dictionary<TableAttribute, int> UpperBounds { get; }
        public Dictionary<TableAttribute, int> LowerBounds { get; }

        public SegmentListMerger(Dictionary<TableAttribute, int> upperBounds, Dictionary<TableAttribute, int> lowerBounds)
        {
            UpperBounds = upperBounds;
            LowerBounds = lowerBounds;
        }

        public List<ISegmentResult> Stitch(Dictionary<TableAttribute, List<ISegmentResult>> dict)
        {
            var newList = new List<ISegmentResult?>();

            if (dict.Keys.Count > 0)
            {
                var firstKey = dict.Keys.First();
                int sourceLowerBound = GetValueFromDictOrAlt(firstKey, LowerBounds, 0);
                int sourceUpperBound = GetValueFromDictOrAlt(firstKey, UpperBounds, dict[firstKey].Count);

                for (int i = 0; i < int.MaxValue; i++)
                {
                    if (i >= sourceLowerBound && i < sourceUpperBound)
                        newList.Insert(i, dict[firstKey][i]);
                    if (i > sourceUpperBound)
                        break;
                }


                foreach (var key in dict.Keys.Skip(1))
                {
                    sourceLowerBound = GetValueFromDictOrAlt(key, LowerBounds, 0);
                    sourceUpperBound = GetValueFromDictOrAlt(key, UpperBounds, dict[key].Count);

                    for (int i = 0; i < newList.Count; i++)
                    {
                        if (i >= sourceLowerBound && i < sourceUpperBound)
                        {
                            if (newList[i] != null)
                            {
                                if (newList[i].IsReferencingTableAttribute(key))
                                {
                                    var thisValue = dict[key][i];
                                    if (thisValue is ValueResult res)
                                        newList[i] = new SegmentResult(newList[i], new ValueResult(
                                            res.TableA,
                                            res.TableB,
                                            res.LeftCount,
                                            1));
                                }
                                else
                                {
                                    var thisValue = dict[key][i];
                                    if (thisValue is ValueResult res)
                                        newList[i] = new SegmentResult(newList[i], new ValueResult(
                                            res.TableA,
                                            res.TableB,
                                            res.LeftCount,
                                            res.RightCount));
                                }
                            }
                        }
                        if (i > sourceUpperBound)
                            break;
                    }
                }
            }

            return newList;
        }

        private static int GetValueFromDictOrAlt(TableAttribute attr, Dictionary<TableAttribute, int> dict, int alt)
        {
            if (dict.ContainsKey(attr))
                return dict[attr];
            return alt;
        }
    }
}
