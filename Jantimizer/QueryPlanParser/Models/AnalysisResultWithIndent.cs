using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseConnector.Models
{
    public class AnalysisResultWithIndent
    {
        public AnalysisResult AnalysisResult { get; set; }
        public int indentation { get; set; }

        public AnalysisResultWithIndent(AnalysisResult analysisResult, int indentation)
        {
            AnalysisResult = analysisResult;
            this.indentation = indentation;
        }
    }
}
