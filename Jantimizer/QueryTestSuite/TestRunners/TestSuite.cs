using DatabaseConnector;
using QueryTestSuite.Models;
using Tools.Helpers;

namespace QueryTestSuite.TestRunners
{
    internal class TestSuite
    {
        public IEnumerable<SuiteData> SuiteDataItems { get; }
        private DateTime TimeStamp;

        public TestSuite(IEnumerable<SuiteData> suiteDataItems, DateTime timeStamp)
        {
            SuiteDataItems = suiteDataItems;
            TimeStamp = timeStamp;
        }

        public async Task RunTests(DirectoryInfo dir)
        {
            foreach(SuiteData suitData in SuiteDataItems)
            {
                if (suitData.ShouldRun)
                {
                    IOHelper.CreateDirIfNotExist(dir.FullName, "Cases/");
                    var caseDir = IOHelper.GetDirectory(dir.FullName, "Cases/");

                    TestRunner runner = new TestRunner(
                        dir.Name,
                        suitData,
                        IOHelper.GetFileVariant(dir, "testSettings", suitData.Name, "json"),
                        IOHelper.GetFileVariantOrNone(dir, "setup", suitData.Name, "sql"),
                        IOHelper.GetFileVariantOrNone(dir, "cleanup", suitData.Name, "sql"),
                        IOHelper.GetInvariantsInDir(caseDir).Select(invariant => IOHelper.GetFileVariant(caseDir, invariant, suitData.Name, "sql")),
                        TimeStamp
                    );

                    await runner.Run(true);
                }
            }
        }
    }
}
