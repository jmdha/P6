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
        public FileInfo SetupFile { get; private set; }
        public FileInfo CleanupFile { get; private set; }
        public IEnumerable<FileInfo> CaseFiles { get; private set; }
        public List<AnalysisResult> Results { get; private set; }

        public TestRunner(DatabaseCommunicator databaseModel, FileInfo setupFile, FileInfo cleanupFile, IEnumerable<FileInfo> caseFiles)
        {
            DatabaseModel = databaseModel;
            SetupFile = setupFile;
            CleanupFile = cleanupFile;
            CaseFiles = caseFiles;
            Results = new List<AnalysisResult>();
        }

        public async Task<List<AnalysisResult>> Run(bool runParallel = false, bool consoleOutput = true)
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

        private void WriteResultToConsole()
        {
            foreach(var analysisResult in Results)
                Console.WriteLine($"Database predicted cardinality: [{(analysisResult.EstimatedCardinality)}], actual: [{analysisResult.ActualCardinality}]");
        }

    }
}
