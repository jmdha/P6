using QueryTestSuite.Connectors;
using QueryTestSuite.Parsers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryTestSuite.Models
{
    internal class TestRunner
    {
        public DatabaseCommunicator DatabaseModel { get; }
        public FileInfo SetupFile { get; set; }
        public FileInfo CleanupFile { get; set; }
        public IEnumerable<FileInfo> CaseFiles { get; set; }

        public TestRunner(DatabaseCommunicator databaseModel, FileInfo setupFile, FileInfo cleanupFile, IEnumerable<FileInfo> caseFiles)
        {
            DatabaseModel = databaseModel;
            SetupFile = setupFile;
            CleanupFile = cleanupFile;
            CaseFiles = caseFiles;
        }

        public async Task<List<AnalysisResult>> Run(bool runParallel = false)
        {
            Console.WriteLine($"Running Cleanup: {CleanupFile}");
            await DatabaseModel.Connector.CallQuery(CleanupFile);

            Console.WriteLine($"Running Setup: {SetupFile}");
            await DatabaseModel.Connector.CallQuery(SetupFile);

            if (runParallel)
                return await RunQueriesParallel();
            else
                return await RunQueriesSerial();
        }

        private async Task<List<AnalysisResult>> RunQueriesParallel()
        {
            var queryAnalysisTasks = new List<Task<DataSet>>();
            foreach (var queryFile in CaseFiles)
            {
                Console.WriteLine($"Spawning Task for: {queryFile}");
                queryAnalysisTasks.Add(DatabaseModel.Connector.AnalyseQuery(queryFile));
            }

            await Task.WhenAll(queryAnalysisTasks);

            var queryAnalysisResults = new List<AnalysisResult>();
            foreach (var task in queryAnalysisTasks)
            {
                queryAnalysisResults.Add(DatabaseModel.Parser.ParsePlan(await task));
            }
            return queryAnalysisResults;
        }

        private async Task<List<AnalysisResult>> RunQueriesSerial()
        {
            var queryAnalysisResults = new List<AnalysisResult>();
            foreach (FileInfo queryFile in CaseFiles)
            {
                Console.WriteLine($"Running {queryFile}");
                queryAnalysisResults.Add(DatabaseModel.Parser.ParsePlan(await DatabaseModel.Connector.AnalyseQuery(queryFile)));
            }
            return queryAnalysisResults;
        }


        public async Task Cleanup()
        {
            await DatabaseModel.Connector.CallQuery(CleanupFile);
        }
    }
}
