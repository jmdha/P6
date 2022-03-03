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
        public DbCommunicationModel CommunicationModel { get; }
        public FileInfo SetupFile { get; set; }
        public FileInfo CleanupFile { get; set; }
        public IEnumerable<FileInfo> CaseFiles { get; set; }

        public TestRunner(DbCommunicationModel databaseModel, FileInfo setupFile, FileInfo cleanupFile, IEnumerable<FileInfo> caseFiles)
        {
            CommunicationModel = databaseModel;
            SetupFile = setupFile;
            CleanupFile = cleanupFile;
            CaseFiles = caseFiles;
        }

        public async Task<List<AnalysisResult>> Run(bool runParallel = true)
        {
            Console.WriteLine($"Running Cleanup: {CleanupFile}");
            await CommunicationModel.Connector.CallQuery(CleanupFile);

            Console.WriteLine($"Running Setup: {SetupFile}");
            await CommunicationModel.Connector.CallQuery(SetupFile);

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
                queryAnalysisTasks.Add(CommunicationModel.Connector.AnalyseQuery(queryFile));
            }

            await Task.WhenAll(queryAnalysisTasks);

            var queryAnalysisResults = new List<AnalysisResult>();
            foreach (var task in queryAnalysisTasks)
            {
                queryAnalysisResults.Add(CommunicationModel.Parser.GetAnalysis(await task));
            }
            return queryAnalysisResults;
        }

        private async Task<List<AnalysisResult>> RunQueriesSerial()
        {
            var queryAnalysisResults = new List<AnalysisResult>();
            foreach (FileInfo queryFile in CaseFiles)
            {
                Console.WriteLine($"Running {queryFile}");
                queryAnalysisResults.Add(CommunicationModel.Parser.GetAnalysis(await CommunicationModel.Connector.AnalyseQuery(queryFile)));
            }
            return queryAnalysisResults;
        }


        public async Task Cleanup()
        {
            await CommunicationModel.Connector.CallQuery(CleanupFile);
        }
    }
}
