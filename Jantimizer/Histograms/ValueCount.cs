using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Histograms
{
    public class ValueCount
    {
        public int Value { get; set; }
        public long Count { get; set; }
        public ValueCount(int value, long count)
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
