using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tools.Helpers
{
    public static class IComparableHelper
    {
        public static bool IsLargerThanOrEqual(this IComparable value1, IComparable value2)
        {
            return value1.CompareTo(value2) >= 0;
        }

        public static bool IsLargerThan(this IComparable value1, IComparable value2)
        {
            return value1.CompareTo(value2) > 0;
        }

        public static bool IsLessThanOrEqual(this IComparable value1, IComparable value2)
        {
            return value1.CompareTo(value2) <= 0;
        }

        public static bool IsLessThan(this IComparable value1, IComparable value2)
        {
            return value1.CompareTo(value2) < 0;
        }

        public static bool IsEqual(this IComparable value1, IComparable value2)
        {
            return value1.CompareTo(value2) == 0;
        }
    }
}
