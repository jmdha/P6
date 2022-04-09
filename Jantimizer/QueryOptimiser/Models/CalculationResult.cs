using Histograms.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryOptimiser.Models
{
    public class CalculationResult
    {
        public long Estimate { get; }
        internal IntermediateTable Table { get; set; }

        internal CalculationResult(long estimate)
        {
            Estimate = estimate;
        }

        internal CalculationResult(long estimate, IntermediateTable table)
        {
            Estimate = estimate;
            Table = table;
        }

        internal CalculationResult(IntermediateTable table)
        {
            Table = table;
            Estimate = Table.GetRowEstimate();
        }
    }
}
