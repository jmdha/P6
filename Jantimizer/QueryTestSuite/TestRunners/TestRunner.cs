using CsvHelper;
using CsvHelper.Configuration;
using DatabaseConnector;
using DatabaseConnector.Connectors;
using Histograms;
using Histograms.Managers;
using QueryPlanParser.Models;
using QueryTestSuite.Models;
using QueryTestSuite.Services;
using System.Data;

namespace QueryTestSuite.TestRunners
{
    internal class TestRunner
    {
        public DBConnectorParser DatabaseModel { get; }
        public FileInfo SetupFile { get; private set; }
        public FileInfo CleanupFile { get; private set; }
        public IEnumerable<FileInfo> CaseFiles { get; private set; }
        public List<TestCase> Results { get; private set; }
        public IHistogramManager<IHistogram, IDbConnector> HistogramManager { get; private set; }
        private CSVWriter csvWriter;

        public TestRunner(DBConnectorParser databaseModel, FileInfo setupFile, FileInfo cleanupFile, IEnumerable<FileInfo> caseFiles, DateTime timeStamp)
        {
            DatabaseModel = databaseModel;
            SetupFile = setupFile;
            CleanupFile = cleanupFile;
            CaseFiles = caseFiles;
            Results = new List<TestCase>();
            csvWriter = new CSVWriter($"Results/{timeStamp.ToString("yyyy/MM/dd/HH.mm.ss")}", "result.csv");
            HistogramManager = new PostgresEquiDepthHistogramManager(databaseModel.Connector.ConnectionString, 10);
        }

        public async Task<List<TestCase>> Run(bool consoleOutput = true, bool saveResult = true)
        {
            Console.WriteLine($"Running Cleanup: {CleanupFile}");
            await DatabaseModel.Connector.CallQuery(CleanupFile);

            Console.WriteLine($"Running Setup: {SetupFile}");
            await DatabaseModel.Connector.CallQuery(SetupFile);
            await HistogramManager.AddHistograms(SetupFile);

            Results = await RunQueriesSerial();

            Console.WriteLine($"Running Cleanup: {CleanupFile}");
            await DatabaseModel.Connector.CallQuery(CleanupFile);

            if (consoleOutput)
                WriteResultToConsole();
            if (saveResult)
                SaveResult();


            return Results;
        }

        private async Task<List<TestCase>> RunQueriesSerial()
        {
            var testCases = new List<TestCase>();
            foreach (FileInfo queryFile in CaseFiles)
            {
                try
                {
                    Console.WriteLine($"Running {queryFile}");
                    DataSet returnSet = await DatabaseModel.Connector.AnalyseQuery(queryFile);
                    AnalysisResult analysisResult = DatabaseModel.Parser.ParsePlan(returnSet);
                    TestCase testCase = new TestCase(queryFile, analysisResult);
                    testCases.Add(testCase);
                }
                catch(Exception ex)
                {
                    Console.WriteLine($"Error! The query file [{queryFile}] failed with the following error:");
                    Console.WriteLine(ex);
                }
            }
            return testCases;
        }

        private void WriteResultToConsole()
        {
            foreach(var testCase in Results)
                Console.WriteLine($"{DatabaseModel.Name} | {testCase.Category} | {testCase.Name} | Database predicted cardinality: [{(testCase.TestResult.EstimatedCardinality)}], actual: [{testCase.TestResult.ActualCardinality}]");
        }

        private void SaveResult()
        {
            csvWriter.Write();
        }

    }
}
