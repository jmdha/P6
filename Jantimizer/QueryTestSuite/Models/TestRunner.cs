using QueryTestSuite.Connectors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryTestSuite.Models
{
    internal class TestRunner
    {
        public DbConnector Connector { get; }
        public FileInfo SetupFile { get; set; }
        public FileInfo CleanupFile { get; set; }
        public IEnumerable<FileInfo> CaseFiles { get; set; }

        public TestRunner(DbConnector connector, FileInfo setupFile, FileInfo cleanupFile, IEnumerable<FileInfo> caseFiles)
        {
            Connector = connector;
            SetupFile = setupFile;
            CleanupFile = cleanupFile;
            CaseFiles = caseFiles;
        }

        public async Task<List<AnalysisResult>> Run(bool runParallel = true)
        {
            Console.WriteLine($"Running Cleanup: {CleanupFile}");
            await Connector.CallQuery(CleanupFile);

            Console.WriteLine($"Running Setup: {SetupFile}");
            await Connector.CallQuery(SetupFile);

            if (runParallel)
                return await RunQueriesParallel();
            else
                return await RunQueriesSerial();
        }

        private async Task<List<AnalysisResult>> RunQueriesParallel()
        {
            var queryAnalysisTasks = new List<Task<AnalysisResult>>();
            foreach (var queryFile in CaseFiles)
            {
                Console.WriteLine($"Spawning Task for: {queryFile}");
                queryAnalysisTasks.Add(Connector.GetAnalysis(queryFile));
            }

            await Task.WhenAll(queryAnalysisTasks);

            var queryAnalysisResults = new List<AnalysisResult>();
            foreach (var task in queryAnalysisTasks)
            {
                queryAnalysisResults.Add(await task);
            }
            return queryAnalysisResults;
        }

        private async Task<List<AnalysisResult>> RunQueriesSerial()
        {
            var queryAnalysisResults = new List<AnalysisResult>();
            foreach (FileInfo queryFile in CaseFiles)
            {
                Console.WriteLine($"Running {queryFile}");
                queryAnalysisResults.Add(await Connector.GetAnalysis(queryFile));
            }
            return queryAnalysisResults;
        }


        public async Task Cleanup()
        {
            await Connector.CallQuery(CleanupFile);
        }
    }
}
