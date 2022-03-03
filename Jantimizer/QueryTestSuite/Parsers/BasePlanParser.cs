using QueryTestSuite.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryTestSuite.Parsers
{
    public abstract class BasePlanParser : IPlanParser
    {
        public AnalysisResult LatestPlan { get; internal set; }

        public abstract AnalysisResult ParsePlan(DataSet planData);

        protected static TimeSpan TimeSpanFromMs(decimal ms) => new TimeSpan((long)Math.Round(ms * 10000)); // 1 million ns per ms, but 1 tick is 100 ns, thus there are 1000000/100=10000 ticks per ms
    }
}
