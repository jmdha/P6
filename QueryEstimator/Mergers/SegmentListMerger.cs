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
                    else
                    {
                        if (i >= sourceUpperBound)
                            break;
                        newList.Add(null);
                    }
                }

                var keyList = new List<TableAttribute>(newList[sourceLowerBound].GetTabelAttributes());
                foreach (var key in dict.Keys.Skip(1))
                {
                    var newLowerBound = GetValueFromDictOrAlt(key, LowerBounds, 0);
                    var newUpperBound = GetValueFromDictOrAlt(key, UpperBounds, dict[key].Count);
                    if (newLowerBound > sourceLowerBound)
                        sourceLowerBound = newLowerBound;
                    if (newUpperBound < sourceUpperBound)
                        sourceUpperBound = newUpperBound;
                    if (dict[key][newLowerBound] != null && dict[key].Count > newLowerBound)
                    {
                        if (dict[key][newLowerBound].IsReferencingTableAttribute(keyList))
                        {
                            for (int i = sourceLowerBound; i < sourceUpperBound; i++)
                            {
                                if (newList.Count > i && newList[i] is ISegmentResult segmentItem)
                                {
                                    newList[i] = new SegmentResult(segmentItem, RemoveReferencedTableCount(dict[key][i], keyList[0]));
                                    foreach (var usedKey in keyList)
                                        newList[i] = new SegmentResult(segmentItem, RemoveReferencedTableCount(newList[i], usedKey));
                                }
                            }
                        }
                        else
                        {
                            for (int i = newLowerBound; i < newUpperBound; i++)
                            {
                                if (newList.Count > i && newList[i] is ISegmentResult segmentItem)
                                    newList[i] = new SegmentResult(segmentItem, dict[key][i]);
                            }
                        }
                        keyList.AddRange(dict[key][newLowerBound].GetTabelAttributes());
                    }
                }
            }

            newList.RemoveAll(x => x == null);

            return newList;
        }

        private ISegmentResult RemoveReferencedTableCount(ISegmentResult result, TableAttribute attr)
        {
            if (result is ValueResult res)
            {
                if (res.TableA != res.TableB)
                {
                    if (res.TableA == attr)
                        res.LeftCount = 1;
                    else if (res.TableB == attr)
                        res.RightCount = 1;
                }
                return res;
            }
            else if (result is SegmentResult seg)
            {
                seg.Left = RemoveReferencedTableCount(seg.Left, attr);
                seg.Right = RemoveReferencedTableCount(seg.Right, attr);
                return seg;
            }
            throw new ArgumentNullException();
        }

        private int GetValueFromDictOrAlt(TableAttribute attr, Dictionary<TableAttribute, int> dict, int alt)
        {
            if (dict.ContainsKey(attr))
                return dict[attr];
            return alt;
        }
    }
}
