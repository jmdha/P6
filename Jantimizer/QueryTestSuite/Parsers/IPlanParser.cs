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
        public Task<AnalysisResult> ParsePlan(DataSet planData);
        public Task<AnalysisResult> ParsePlan(Task<DataSet> planData);
    }
}
