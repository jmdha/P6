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
        public bool DoCleanup { get; set; }
        public bool DoSetup { get; set; }
        public bool DoMakeHistograms { get; set; }
        public ConnectionProperties Properties { get; set; }

        public TestSettings()
        {
            DoCleanup = true;
            DoSetup = true;
            DoMakeHistograms = true;
            Properties = new ConnectionProperties();
        }

        public TestSettings(bool doCleanup, bool doSetup, bool doMakeHistograms, ConnectionProperties properties)
        {
            DoCleanup = doCleanup;
            DoSetup = doSetup;
            DoMakeHistograms = doMakeHistograms;
            Properties = properties;
        }

        public void Update(TestSettings settings)
        {
            DoCleanup = settings.DoCleanup;
            DoSetup = settings.DoSetup;
            DoMakeHistograms = settings.DoMakeHistograms;
            Properties.Update(settings.Properties);
        }
    }
}
