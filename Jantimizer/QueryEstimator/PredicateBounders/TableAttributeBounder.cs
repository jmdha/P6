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
            var allSourceSegments = GetAllSegmentsForAttribute(source);
            int currentSourceLowerBound = GetLowerBoundOrAlt(source, 0);
            int newSourceLowerBound = currentSourceLowerBound;
            int currentSourceUpperBound = GetUpperBoundOrAlt(source, allSourceSegments.Count - 1);
            int newSourceUpperBound = currentSourceUpperBound;

            IHistogramSegmentationComparative lastEqual = allSourceSegments[currentSourceLowerBound];

            if (currentSourceLowerBound != currentSourceUpperBound)
            {
                if (type == ComparisonType.Type.Equal)
                    currentSourceLowerBound++;

                bool foundAny = false;
                for (int i = currentSourceLowerBound; i <= currentSourceUpperBound; i++)
                {
                    bool isAny = false;
                    switch (type)
                    {
                        case ComparisonType.Type.Equal:
                            isAny = allSourceSegments[i].IsAnySmallerThanNoAlias(compare) && lastEqual.IsAnyLargerThanNoAlias(compare);
                            lastEqual = allSourceSegments[i];
                            break;
                        case ComparisonType.Type.More:
                        case ComparisonType.Type.EqualOrMore:
                            isAny = allSourceSegments[i].IsAnySmallerThanNoAlias(compare);
                            break;
                        case ComparisonType.Type.Less:
                        case ComparisonType.Type.EqualOrLess:
                            isAny = allSourceSegments[i].IsAnyLargerThanNoAlias(compare);
                            break;
                    }

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
                        foundAny = true;
                }

                AddToUpperBoundIfNotThere(source, newSourceUpperBound);
                AddToLowerBoundIfNotThere(source, newSourceLowerBound);
            }

            return new PredicateBoundResult<TableAttribute>(this, source, compare, type, newSourceUpperBound, newSourceLowerBound);
        }
    }
}
