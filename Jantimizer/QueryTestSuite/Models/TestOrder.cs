using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryTestSuite.Models
{
    internal class TestOrder
    {
        public List<string> Order { get; set; }

        public TestOrder(List<string> order)
        {
            Order = order;
        }
    }
}
