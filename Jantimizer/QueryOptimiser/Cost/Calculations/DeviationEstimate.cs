using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryOptimiser.Cost.Calculations
{
    internal static class DeviationEstimate
    {
        internal static double GetCertainty(double standardDeviation, double range)
        {
            double bucketCertainty;
            if (range == 0 || standardDeviation == 0)
                bucketCertainty = 1;
            else
                bucketCertainty = standardDeviation / range;
            return bucketCertainty;
        }

        internal static double GetComparativeCertainty(double certainty, double comparisonCertainty)
        {
            if (comparisonCertainty == 0)
                throw new ArgumentOutOfRangeException("Error, cannot divide certainty by 0!");
            double comparativeCertainty = certainty / comparisonCertainty;
            if (comparativeCertainty > 1)
                comparativeCertainty = 1 / comparativeCertainty;
            return comparativeCertainty;
        }
    }
}
