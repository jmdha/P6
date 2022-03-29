using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryPlanParser.Models
{
    internal class AnalysisResultWithIndent
    {
        public AnalysisResultQueryTree AnalysisResult { get; }
        public int Indentation { get; }

        public AnalysisResultWithIndent(AnalysisResultQueryTree analysisResult, int indentation)
        {
            AnalysisResult = analysisResult;
            Indentation = indentation;
        }
    }
}
