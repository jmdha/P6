using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExperimentSuite.Models.ExperimentParsing
{
    internal class ExperimentList
    {
        public List<ExperimentData> Experiments { get; set; }

        public ExperimentList(List<ExperimentData> experiments)
        {
            Experiments = experiments;
        }
    }
}
