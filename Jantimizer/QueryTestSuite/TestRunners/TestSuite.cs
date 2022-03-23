using DatabaseConnector;
using Konsole;
using QueryTestSuite.Models;
using QueryTestSuite.Exceptions;
using QueryTestSuite.Services;

namespace QueryTestSuite.TestRunners
{
    internal class TestSuite
    {
        public IEnumerable<SuiteData> SuiteDataItems { get; }
        private DateTime TimeStamp;
        private WindowManager WindowManager { get; set;}

        public TestSuite(IEnumerable<SuiteData> suiteDataItems, DateTime timeStamp, string name)
        {
            SuiteDataItems = suiteDataItems;
            TimeStamp = timeStamp;
            WindowManager = new WindowManager();
            WindowManager.GenerateConsoleLayout(name, suiteDataItems.ToList());
        }

        private void GenerateStatusWindow(IConsole parent, List<SuiteData> suites, int suitesToRun) {
            if (suitesToRun == 1) {
                Consoles.Add(suites[0].Name, parent.OpenBox(suites[0].Name));
            } else if (suitesToRun > 2) {
                Consoles.Add(suites[0].Name, parent.SplitLeft(suites[0].Name));
                Consoles.Add(suites[1].Name, parent.SplitRight(suites[1].Name));
            }
        }

        private void GenerateReportsWindow(IConsole parent, List<SuiteData> suites, int reportSize) {
            List<Split> splits = new List<Split>();
            List<string> splitNames = new List<string>();
            foreach (var suite in suites) {
                if (suite.ShouldRun) {
                    splits.Add(new Split(reportSize, suite.Name));
                    splitNames.Add(suite.Name);
                }
                    
            }
            var reports = parent.SplitRows(splits.ToArray());
            for (int i = 0; i < reports.Count(); i++) {
                Consoles.Add(splitNames[i], reports[i]);
            }
        }

        public async Task RunTests(DirectoryInfo dir)
        {
            List<Task> testRunnerTask = new List<Task>();
            foreach(SuiteData suitData in SuiteDataItems)
            {
                if (suitData.ShouldRun)
                {
                    Task task = Task.Run(async () =>
                    {
                        var testRunners = new List<TestRunner>();
                        var testRuns = new List<Task<List<TestCaseResult>>>();

                        if (!Directory.Exists(Path.Join(dir.FullName, "Cases/")))
                            Directory.CreateDirectory(Path.Join(dir.FullName, "Cases/"));
                        var caseDir = new DirectoryInfo(Path.Join(dir.FullName, "Cases/"));

                        TestRunner runner = new TestRunner(
                            suitData,
                            GetVariant(dir, "testSettings", suitData.Name, "json"),
                            GetVariant(dir, "setup", suitData.Name),
                            GetVariant(dir, "cleanup", suitData.Name),
                            GetInvariantsInDir(caseDir).Select(invariant => GetVariant(caseDir, invariant, suitData.Name)),
                            TimeStamp,
                            Consoles[suitData.Name]
                        );
                        testRunners.Add(runner);
                        testRuns.Add(runner.Run(true));
                        await Task.WhenAll(testRuns);
                    });
                    testRunnerTask.Add(task);
                }
            }
            await Task.WhenAll(testRunnerTask);
        }

        private FileInfo GetVariant(DirectoryInfo dir, string name, string type, string ext = "sql")
        {
            var specific = new FileInfo(Path.Combine(dir.FullName, $"{name}.{type}.{ext}"));

            if(specific.Exists)
                return specific;

            var generic = new FileInfo(Path.Combine(dir.FullName, $"{name}.{ext}"));

            if(generic.Exists)
                return generic;

            return new FileInfo("None");
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
