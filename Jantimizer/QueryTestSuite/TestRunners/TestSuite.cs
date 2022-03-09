using QueryTestSuite.Models;
using QueryTestSuite.TestRunners;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryTestSuite.Connectors
{
    internal class TestSuite
    {
        public IEnumerable<DatabaseCommunicator> DatabaseModels { get; }
        private DateTime TimeStamp;

        public TestSuite(IEnumerable<DatabaseCommunicator> databaseModels, DateTime timeStamp)
        {
            DatabaseModels = databaseModels;
            TimeStamp = timeStamp;
        }

        public async Task RunTests(DirectoryInfo dir)
        {
            var testRunners = new List<TestRunner>();
            var testRuns = new List<Task<List<TestCase>>>();

            foreach(DatabaseCommunicator databaseModel in DatabaseModels)
            {
                var caseDir = new DirectoryInfo(Path.Join(dir.FullName, "Cases/"));

                TestRunner runner = new TestRunner(
                    databaseModel,
                    GetVariant(dir, "setup", databaseModel.Name),
                    GetVariant(dir, "cleanup", databaseModel.Name),
                    GetInvariantsInDir(caseDir).Select(invariant => GetVariant(caseDir, invariant, databaseModel.Name)),
                    TimeStamp
                );
                testRunners.Add(runner);
                Console.WriteLine("Running tests");
                testRuns.Add(runner.Run(true));
            }

            await Task.WhenAll(testRuns);
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
