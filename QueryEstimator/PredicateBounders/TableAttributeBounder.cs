using Histograms;
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
            int newSourceLowerBound = GetLowerBoundOrAlt(source, 0);
            int newSourceUpperBound = GetUpperBoundOrAlt(source, allSourceSegments.Count);

            if (type == ComparisonType.Type.More)
            {
                bool foundAny = false;
                for (int i = newSourceUpperBound - 1; i >= newSourceLowerBound; i--)
                {
                    bool isAny = allSourceSegments[i].IsAnySmallerThanNoAlias(compare);

                    if (!isAny && !foundAny)
                    {
                        newSourceUpperBound = i;
                        continue;
                    }
                    else if (!isAny)
                    {
                        newSourceLowerBound = i;
                        break;
                    }
                    else
                    {
                        foundAny = true;
                    }
                }
            }
            if (type == ComparisonType.Type.Less)
            {
                bool foundAny = false;
                for (int i = newSourceLowerBound; i < newSourceUpperBound; i++)
                {
                    bool isAny = allSourceSegments[i].IsAnyLargerThanNoAlias(compare);

                    if (!isAny && foundAny)
                    {
                        newSourceUpperBound = i;
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
            }

            AddToUpperBoundIfNotThere(source, newSourceUpperBound);
            AddToLowerBoundIfNotThere(source, newSourceLowerBound);

            return new PredicateBoundResult<TableAttribute>(this, source, compare, type, newSourceUpperBound, newSourceLowerBound);
        }       
    }
}
