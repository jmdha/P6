using DatabaseConnector;
using QueryTestSuite.Models;

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
                    var testRunners = new List<TestRunner>();
                    var testRuns = new List<Task<List<TestCaseResult>>>();

                    if (!Directory.Exists(Path.Join(dir.FullName, "Cases/")))
                        Directory.CreateDirectory(Path.Join(dir.FullName, "Cases/"));
                    var caseDir = new DirectoryInfo(Path.Join(dir.FullName, "Cases/"));

                    TestRunner runner = new TestRunner(
                        suitData,
                        GetVariant(dir, "setup", suitData.Name),
                        GetVariant(dir, "cleanup", suitData.Name),
                        GetInvariantsInDir(caseDir).Select(invariant => GetVariant(caseDir, invariant, suitData.Name)),
                        TimeStamp
                    );
                    testRunners.Add(runner);
                    testRuns.Add(runner.Run(true));

                    await Task.WhenAll(testRuns);
                }
            }
        }

        private FileInfo GetVariant(DirectoryInfo dir, string name, string type)
        {
            var specific = new FileInfo(Path.Combine(dir.FullName, $"{name}.{type}.sql"));

            if(specific.Exists)
                return specific;

            var generic = new FileInfo(Path.Combine(dir.FullName, $"{name}.sql"));

            if(generic.Exists)
                return generic;

            throw new FileNotFoundException($"Could not find '{name}' of type '{type}' in '{dir.FullName}'");
        }

        List<string> GetInvariantsInDir(DirectoryInfo dir)
        {
            // Every filename, until first '.', unique
            return dir.GetFiles()
                .Select(x => x.Name.Split('.')[0])
                .GroupBy(x => x)
                .Select(x => x.First())
                .ToList();
        }
    }
}
