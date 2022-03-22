using DatabaseConnector;
using Konsole;
using QueryTestSuite.Models;

namespace QueryTestSuite.TestRunners
{
    internal class TestSuite
    {
        public IEnumerable<SuiteData> SuiteDataItems { get; }
        private DateTime TimeStamp;
        private IConsole PrimaryConsole;
        private Dictionary<string, IConsole> Consoles = new Dictionary<string, IConsole>();

        public TestSuite(IEnumerable<SuiteData> suiteDataItems, DateTime timeStamp, string name)
        {
            SuiteDataItems = suiteDataItems;
            TimeStamp = timeStamp;
            PrimaryConsole = Window.OpenBox(name, 220, 50);
            List<SuiteData> data = SuiteDataItems.ToList();
            Consoles.Add(data[0].Name, PrimaryConsole.SplitLeft(data[0].Name));
            Consoles.Add(data[1].Name, PrimaryConsole.SplitRight(data[1].Name));
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
