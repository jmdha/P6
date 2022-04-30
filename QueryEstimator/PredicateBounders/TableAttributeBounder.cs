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
                    long newInnerResult = (long)allSourceSegments[i].GetCountSmallerThanNoAlias(compare);

                    if (newInnerResult == 0 && !foundAny)
                    {
                        newSourceUpperBound = i;
                        continue;
                    }
                    else if (newInnerResult == 0)
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
                    long newInnerResult = (long)allSourceSegments[i].GetCountLargerThanNoAlias(compare);

                    if (newInnerResult == 0 && foundAny)
                    {
                        newSourceUpperBound = i;
                        break;
                    }
                    else if (newInnerResult == 0)
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
