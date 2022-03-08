using QueryTestSuite.TestRunners;
using System.Data;

namespace QueryTestSuite.Models
{
    internal class TestRunner
    {
        public DatabaseCommunicator DatabaseModel { get; }
        public FileInfo SetupFile { get; private set; }
        public FileInfo CleanupFile { get; private set; }
        public IEnumerable<FileInfo> CaseFiles { get; private set; }
        public List<TestCase> Results { get; private set; }

        public TestRunner(DatabaseCommunicator databaseModel, FileInfo setupFile, FileInfo cleanupFile, IEnumerable<FileInfo> caseFiles)
        {
            DatabaseModel = databaseModel;
            SetupFile = setupFile;
            CleanupFile = cleanupFile;
            CaseFiles = caseFiles;
            Results = new List<TestCase>();
        }

        public async Task<List<TestCase>> Run(bool runParallel = false, bool consoleOutput = true)
        {
            Console.WriteLine($"Running Cleanup: {CleanupFile}");
            await DatabaseModel.Connector.CallQuery(CleanupFile);

            Console.WriteLine($"Running Setup: {SetupFile}");
            await DatabaseModel.Connector.CallQuery(SetupFile);

            if (runParallel)
                Results = await RunQueriesParallel();
            else
                Results = await RunQueriesSerial();

            Console.WriteLine($"Running Cleanup: {CleanupFile}");
            await DatabaseModel.Connector.CallQuery(CleanupFile);

            if (consoleOutput)
                WriteResultToConsole();

            return Results;
        }

        private async Task<List<TestCase>> RunQueriesParallel()
        {
            List<Task<TestCase>> testTasks = new List<Task<TestCase>>();
            foreach (FileInfo queryFile in CaseFiles)
            {
                Console.WriteLine($"Spawning Task for: {queryFile}");
                Task<TestCase> testTask = GenerateAndRunTestCase(queryFile);
                testTasks.Add(testTask);
            }

            await Task.WhenAll(testTasks);

            var testCases = new List<TestCase>();
            foreach (var testTask in testTasks)
            {
                testCases.Add(testTask.Result);
            }
            return testCases;
        }

        private async Task<List<TestCase>> RunQueriesSerial()
        {
            var testCases = new List<TestCase>();
            foreach (FileInfo queryFile in CaseFiles)
            {
                Console.WriteLine($"Running {queryFile}");
                TestCase testCase = await GenerateAndRunTestCase(queryFile);
                testCases.Add(testCase);
            }
            return testCases;
        }

        private Task<TestCase> GenerateAndRunTestCase(FileInfo queryFile)
        {
            string testName = queryFile.Name;
            string testCategory;
            try
            {
                testCategory = queryFile.Directory.Parent.Name;
            } catch (Exception ex)
            {
                testCategory = "N/A";
                Console.WriteLine($"Could not get test caregory - {queryFile}");
            }
            
            TestCase testCase = new TestCase(testName, testCategory);
            return testCase.Run(DatabaseModel.Parser.ParsePlan(DatabaseModel.Connector.AnalyseQuery(queryFile)));
        }

        private void WriteResultToConsole()
        {
            foreach(var testCase in Results)
                Console.WriteLine($"{DatabaseModel.Name} | {testCase.Category} | {testCase.Name} | Database predicted cardinality: [{(testCase.AnalysisResult.EstimatedCardinality)}], actual: [{testCase.AnalysisResult.ActualCardinality}]");
        }

    }
}
