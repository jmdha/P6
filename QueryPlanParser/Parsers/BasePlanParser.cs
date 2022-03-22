using QueryPlanParser.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("QueryPlanParserTests")]

namespace QueryPlanParser.Parsers
{
    public abstract class BasePlanParser : IPlanParser
    {
        public abstract AnalysisResult ParsePlan(DataSet planData);

        internal static TimeSpan TimeSpanFromMs(decimal ms) => new TimeSpan((long)Math.Round(ms * 10000)); // 1 million ns per ms, but 1 tick is 100 ns, thus there are 1000000/100=10000 ticks per ms
    }
}
