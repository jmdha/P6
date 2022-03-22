using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.Models;

namespace QueryTestSuite.Models
{
    public class TestSettings
    {
        public bool? DoPreCleanup { get; set; }
        public bool? DoSetup { get; set; }
        public bool? DoPostCleanup { get; set; }
        public bool? DoMakeHistograms { get; set; }
        public bool? DoRunTests { get; set; }
        public bool? DoMakeReport { get; set; }
        public ConnectionProperties? Properties { get; set; }

        public TestSettings()
        {
        }

        public TestSettings(bool? doPreCleanup, bool? doSetup, bool? doPostCleanup, bool? doMakeHistograms, bool? doRunTests, bool? doMakeReport, ConnectionProperties? properties)
        {
            DoPreCleanup = doPreCleanup;
            DoSetup = doSetup;
            DoPostCleanup = doPostCleanup;
            DoMakeHistograms = doMakeHistograms;
            DoRunTests = doRunTests;
            DoMakeReport = doMakeReport;
            Properties = properties;
        }

        public void Update(TestSettings settings)
        {
            DoPreCleanup = settings.DoPreCleanup;
            DoSetup = settings.DoSetup;
            DoPostCleanup = settings.DoPostCleanup;
            DoMakeHistograms = settings.DoMakeHistograms;
            DoRunTests = settings.DoRunTests;
            DoMakeReport = settings.DoMakeReport;
            if (Properties != null && settings.Properties != null)
                Properties.Update(settings.Properties);
        }
    }
}
