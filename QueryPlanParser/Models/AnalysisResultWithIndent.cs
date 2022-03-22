using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryPlanParser.Models
{
    internal class AnalysisResultWithIndent
    {
        public AnalysisResult AnalysisResult { get; }
        public int Indentation { get; }

        public AnalysisResultWithIndent(AnalysisResult analysisResult, int indentation)
        {
            AnalysisResult = analysisResult;
            Indentation = indentation;
        }
    }
}
