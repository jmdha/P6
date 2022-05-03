using Histograms;
using Histograms.Models;
using QueryEstimator.Models;
using QueryEstimator.Models.BoundResults;
using QueryEstimator.SegmentHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.Helpers;
using Tools.Models.JsonModels;

namespace QueryEstimator.PredicateBounders
{
    public class SimpleFilterBounder : BaseSegmentBoundsHandler, IPredicateBounder<IComparable>
    {
        private IHistogramSegmentationComparative? _lastEqual = null;
        public SimpleFilterBounder(Dictionary<TableAttribute, int> upperBounds, Dictionary<TableAttribute, int> lowerBounds, IHistogramManager histogramManager) : base(upperBounds, lowerBounds, histogramManager)
        {
        }

        public IPredicateBoundResult<IComparable> Bound(TableAttribute source, IComparable compare, ComparisonType.Type type)
        {
            // Get segments and lower/upper bounds for the source attribute
            var allSourceSegments = GetAllSegmentsForAttribute(source);
            int currentSourceLowerBound = GetLowerBoundOrAlt(source, 0);
            int newSourceLowerBound = currentSourceLowerBound;
            int currentSourceUpperBound = GetUpperBoundOrAlt(source, allSourceSegments.Count - 1);
            int newSourceUpperBound = currentSourceUpperBound;

            // Initialise the new source bounds for edge cases
            if (type == ComparisonType.Type.More || type == ComparisonType.Type.EqualOrMore)
            {
                newSourceLowerBound = currentSourceUpperBound;
                newSourceUpperBound = currentSourceUpperBound;
            } else if (type == ComparisonType.Type.Less || type == ComparisonType.Type.EqualOrLess)
            {
                newSourceLowerBound = 0;
                newSourceUpperBound = -1;
            }   

            // Only check for new bounds if the bound have not already been reduced to the max
            if (currentSourceLowerBound < currentSourceUpperBound)
            {
                // Convert the type to compare against to be the same as the one in the segments (assuming its correct)
                compare = ConvertCompareTypes(allSourceSegments[currentSourceLowerBound], compare);

                if (type == ComparisonType.Type.Equal)
                {
                    _lastEqual = allSourceSegments[currentSourceLowerBound];
                    currentSourceLowerBound++;
                }

                // Check within the bounds until a given predicate is no longer correct
                bool exitSentinel = false;
                for (int i = currentSourceLowerBound; i <= currentSourceUpperBound; i++)
                {
                    switch (type)
                    {
                        case ComparisonType.Type.More:
                            newSourceLowerBound = i;
                            if (allSourceSegments[i].LowestValue.IsLargerThan(compare))
                                exitSentinel = true;
                            break;
                        case ComparisonType.Type.EqualOrMore:
                            newSourceLowerBound = i;
                            if (allSourceSegments[i].LowestValue.IsLargerThanOrEqual(compare))
                                exitSentinel = true;
                            break;
                        case ComparisonType.Type.Less:
                            if (allSourceSegments[i].LowestValue.IsLargerThanOrEqual(compare))
                            {
                                exitSentinel = true;
                                break;
                            }
                            newSourceUpperBound = i;
                            break;
                        case ComparisonType.Type.EqualOrLess:
                            if (allSourceSegments[i].LowestValue.IsLargerThan(compare))
                            {
                                exitSentinel = true;
                                break;
                            }
                            newSourceUpperBound = i;
                            break;
                        case ComparisonType.Type.Equal:
                            if (allSourceSegments[i].LowestValue.IsLargerThanOrEqual(compare))
                            {
                                newSourceUpperBound = i;
                                exitSentinel = true;
                                break;
                            }
                            if (_lastEqual != null && _lastEqual.LowestValue != allSourceSegments[i].LowestValue)
                                newSourceLowerBound = i;

                            _lastEqual = allSourceSegments[i];
                            break;
                    }
                    if (exitSentinel)
                        break;
                }

                // Add to dictionary if not there
                AddOrReduceUpperBound(source, newSourceUpperBound);
                AddOrReduceLowerBound(source, newSourceLowerBound);
            }

            // Return a new bound result with the new upper and lower bounds
            return new PredicateBoundResult<IComparable>(this, source, compare, type, newSourceUpperBound, newSourceLowerBound);
        }

        internal IComparable ConvertCompareTypes(IHistogramSegmentationComparative segment, IComparable compare)
        {
            var compType = compare.GetType();
            var valueType = segment.LowestValue.GetType();
            if (compType != valueType)
                return (IComparable)Convert.ChangeType(compare, valueType);
            return compare;
        }
    }
}
