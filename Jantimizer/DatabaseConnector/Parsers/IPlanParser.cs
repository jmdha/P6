using DatabaseConnector.Connectors;
using DatabaseConnector.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseConnector.Parsers
{
    public interface IPlanParser
    {
        public AnalysisResult ParsePlan(DataSet planData);
    }
}
