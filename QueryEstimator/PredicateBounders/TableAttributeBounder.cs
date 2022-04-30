using Histograms;
using Histograms.Models;
using QueryEstimator.Models;
using QueryEstimator.Models.BoundResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.Models.JsonModels;

namespace QueryEstimator.PredicateBounders
{
    public class TableAttributeBounder : BasePredicateBounder<TableAttribute>
    {
        public TableAttributeBounder(Dictionary<TableAttribute, int> upperBounds, Dictionary<TableAttribute, int> lowerBounds, IHistogramManager histogramManager) : base(upperBounds, lowerBounds, histogramManager)
        {
        }

        public override IPredicateBoundResult<TableAttribute> Bound(TableAttribute source, TableAttribute compare, ComparisonType.Type type)
        {
            switch (type)
            {
                case ComparisonType.Type.Equal:
                    return TraverseFromBottomEqual(source, compare, type);
                case ComparisonType.Type.More:
                case ComparisonType.Type.EqualOrMore:
                    return TraverseFromTop(source, compare, type);
                case ComparisonType.Type.Less:
                case ComparisonType.Type.EqualOrLess:
                    return TraverseFromBottom(source, compare, type);
            }
            throw new Exception("Impossible predicate type!");
        }

        private IPredicateBoundResult<TableAttribute> TraverseFromBottomEqual(TableAttribute source, TableAttribute compare, ComparisonType.Type type)
        {
            var allSourceSegments = GetAllSegmentsForAttribute(source);
            int newSourceLowerBound = GetLowerBoundOrAlt(source, 0);
            int newSourceUpperBound = GetUpperBoundOrAlt(source, allSourceSegments.Count - 1);

            if (newSourceLowerBound != newSourceUpperBound)
            {
                IHistogramSegmentationComparative lastEqual = allSourceSegments[newSourceLowerBound];

                bool foundAny = false;
                for (int i = newSourceLowerBound + 1; i <= newSourceUpperBound; i++)
                {
                    bool isAny = allSourceSegments[i].IsAnySmallerThanNoAlias(compare) && lastEqual.IsAnyLargerThanNoAlias(compare);
                    lastEqual = allSourceSegments[i];

                    if (!isAny && foundAny)
                    {
                        newSourceUpperBound = i;
                        break;
                    }
                    else
                    {
                        if (!foundAny)
                            newSourceLowerBound = i - 1;
                        foundAny = true;
                    }
                }

                AddToUpperBoundIfNotThere(source, newSourceUpperBound);
                AddToLowerBoundIfNotThere(source, newSourceLowerBound);
            }

            return new PredicateBoundResult<TableAttribute>(this, source, compare, type, newSourceUpperBound, newSourceLowerBound);
        }

        private IPredicateBoundResult<TableAttribute> TraverseFromTop(TableAttribute source, TableAttribute compare, ComparisonType.Type type)
        {
            var allSourceSegments = GetAllSegmentsForAttribute(source);
            int newSourceLowerBound = GetLowerBoundOrAlt(source, 0);
            int newSourceUpperBound = GetUpperBoundOrAlt(source, allSourceSegments.Count - 1);

            if (newSourceLowerBound != newSourceUpperBound)
            {
                bool foundAny = false;
                for (int i = newSourceUpperBound; i >= newSourceLowerBound; i--)
                {
                    bool isAny = allSourceSegments[i].IsAnySmallerThanNoAlias(compare);

                    if (!isAny && foundAny)
                    {
                        newSourceLowerBound = i;
                        break;
                    }
                    else if (!isAny)
                    {
                        if (!foundAny)
                            newSourceUpperBound = i - 1;
                        continue;
                    }
                    else
                    {
                        foundAny = true;
                    }
                }

                AddToUpperBoundIfNotThere(source, newSourceUpperBound);
                AddToLowerBoundIfNotThere(source, newSourceLowerBound);
            }

            return new PredicateBoundResult<TableAttribute>(this, source, compare, type, newSourceUpperBound, newSourceLowerBound);
        }

        private IPredicateBoundResult<TableAttribute> TraverseFromBottom(TableAttribute source, TableAttribute compare, ComparisonType.Type type)
        {
            var allSourceSegments = GetAllSegmentsForAttribute(source);
            int newSourceLowerBound = GetLowerBoundOrAlt(source, 0);
            int newSourceUpperBound = GetUpperBoundOrAlt(source, allSourceSegments.Count - 1);

            if (newSourceLowerBound != newSourceUpperBound)
            {
                bool foundAny = false;
                for (int i = newSourceLowerBound; i <= newSourceUpperBound; i++)
                {
                    bool isAny = allSourceSegments[i].IsAnyLargerThanNoAlias(compare);

                    if (!isAny && foundAny)
                    {
                        newSourceUpperBound = i - 1;
                        break;
                    }
                    else if (!isAny)
                    {
                        newSourceLowerBound = i;
                        continue;
                    }
                    else
                    {
                        foundAny = true;
                    }
                }

                AddToUpperBoundIfNotThere(source, newSourceUpperBound);
                AddToLowerBoundIfNotThere(source, newSourceLowerBound);
            }

            return new PredicateBoundResult<TableAttribute>(this, source, compare, type, newSourceUpperBound, newSourceLowerBound);
        }
    }
}
