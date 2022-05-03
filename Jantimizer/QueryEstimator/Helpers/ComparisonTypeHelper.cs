using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.Models.JsonModels;

namespace QueryEstimator.Helpers
{
    internal static class ComparisonTypeHelper
    {
        internal static ComparisonType.Type InvertType(ComparisonType.Type fromType)
        {
            switch (fromType)
            {
                case ComparisonType.Type.Less: return ComparisonType.Type.EqualOrMore;
                case ComparisonType.Type.More: return ComparisonType.Type.EqualOrLess;
                case ComparisonType.Type.EqualOrLess: return ComparisonType.Type.More;
                case ComparisonType.Type.EqualOrMore: return ComparisonType.Type.Less;
                case ComparisonType.Type.Equal: return ComparisonType.Type.Equal;
                default:
                    throw new Exception("Impossible comparison type");
            }
        }
    }
}
