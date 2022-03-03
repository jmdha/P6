using QueryTestSuite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryTestSuite.Connectors
{
    internal class TestSuite
    {
        public IEnumerable<DbCommunicationModel> DatabaseModels { get; }

        public TestSuite(IEnumerable<DbCommunicationModel> connectors)
        {
            DatabaseModels = connectors;
        }

        public async Task RunTests(DirectoryInfo dir)
        {
            var testRuns = new List<Task<List<AnalysisResult>>>();
            var testRunners = new List<TestRunner>();

            foreach(DbCommunicationModel databaseModel in DatabaseModels)
            {
                var caseDir = new DirectoryInfo(Path.Join(dir.FullName, "cases/"));

                TestRunner runner = new TestRunner(
                    databaseModel,
                    GetVariant(dir, "setup", databaseModel.Name),
                    GetVariant(dir, "cleanup", databaseModel.Name),
                    GetInvariantsInDir(caseDir).Select(invariant => GetVariant(caseDir, invariant, databaseModel.Name))
                );
                testRuns.Add(runner.Run());
                testRunners.Add(runner);
            }

            await Task.WhenAll(testRuns);
            Console.WriteLine("Cleaning up");
            foreach (var runner in testRunners)
                await runner.Cleanup();
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
