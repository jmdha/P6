using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tools.Helpers
{
    public static class ListHelper
    {
        public static List<T> GetRange<T>(this List<T> list, Range range)
        {
            return list.GetRange(range.Start.Value, range.End.Value - range.Start.Value);
        }
    }
}
