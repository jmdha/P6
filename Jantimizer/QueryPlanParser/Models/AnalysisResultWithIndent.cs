using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryPlanParser.Models
{
    internal class AnalysisResultWithIndent
    {
        public AnalysisResultQueryTree AnalysisQueryTree { get; }
        public int Indentation { get; }

        public AnalysisResultWithIndent(AnalysisResultQueryTree queryTree, int indentation)
        {
            AnalysisQueryTree = queryTree;
            Indentation = indentation;
        }
    }
}
