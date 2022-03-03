using QueryTestSuite.Connectors;
using QueryTestSuite.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryTestSuite.Parsers
{
    public interface IPlanParser
    {
        public AnalysisResult LatestPlan { get; }

        public AnalysisResult ParsePlan(DataSet planData);
    }
}
