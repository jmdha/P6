using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milestoner.Models
{
    public class ValueCount
    {
        public IComparable Value { get; set; }
        public long Count { get; set; }
        public ValueCount(IComparable value, long count)
        {
            Value = value;
            Count = count;
        }

        public override string ToString()
        {
            return $"|{{value={Value}}}|={Count}";
        }
    }
}