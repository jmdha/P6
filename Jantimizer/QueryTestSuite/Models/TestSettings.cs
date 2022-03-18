using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryTestSuite.Models
{
    public class TestSettings
    {
        public bool DoCleanup { get; set; }
        public bool DoSetup { get; set; }
        public bool DoMakeHistograms { get; set; }
        public string Database { get; set; }
        public string Schema { get; set; }

        public TestSettings()
        {
            DoCleanup = true;
            DoSetup = true;
            DoMakeHistograms = true;
            Database = "";
            Schema = "";
        }

        public TestSettings(bool doCleanup, bool doSetup, bool doMakeHistograms, string database, string schema)
        {
            DoCleanup = doCleanup;
            DoSetup = doSetup;
            DoMakeHistograms = doMakeHistograms;
            Database = database;
            Schema = schema;
        }
    }
}
