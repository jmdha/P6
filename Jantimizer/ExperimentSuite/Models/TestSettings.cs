using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.Models;

namespace ExperimentSuite.Models
{
    public class TestSettings
    {
        public bool? DoPreCleanup { get; set; }
        public bool? DoSetup { get; set; }
        public bool? DoInserts { get; set; }
        public bool? DoAnalyse { get; set; }
        public bool? DoPostCleanup { get; set; }
        public bool? DoMakeHistograms { get; set; }
        public bool? DoRunTests { get; set; }
        public bool? DoMakeReport { get; set; }
        public bool? DoMakeTimeReport { get; set; }
        public ConnectionProperties? Properties { get; set; }

        public TestSettings()
        {
        }

        public TestSettings(bool? doPreCleanup, bool? doSetup, bool? doInserts, bool? doAnalyse, bool? doPostCleanup, bool? doMakeHistograms, bool? doRunTests, bool? doMakeReport, bool? doMakeTimeReport, ConnectionProperties? properties)
        {
            DoPreCleanup = doPreCleanup;
            DoSetup = doSetup;
            DoInserts = doInserts;
            DoAnalyse = doAnalyse;
            DoPostCleanup = doPostCleanup;
            DoMakeHistograms = doMakeHistograms;
            DoRunTests = doRunTests;
            DoMakeReport = doMakeReport;
            DoMakeTimeReport = doMakeTimeReport;
            Properties = properties;
        }

        public void Update(TestSettings settings)
        {
            DoPreCleanup = settings.DoPreCleanup;
            DoSetup = settings.DoSetup;
            DoInserts = settings.DoInserts;
            DoAnalyse = settings.DoAnalyse;
            DoPostCleanup = settings.DoPostCleanup;
            DoMakeHistograms = settings.DoMakeHistograms;
            DoRunTests = settings.DoRunTests;
            DoMakeReport = settings.DoMakeReport;
            DoMakeTimeReport = settings.DoMakeTimeReport;
            if (Properties != null && settings.Properties != null)
                Properties.Update(settings.Properties);
        }
    }
}
