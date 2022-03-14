using CsvHelper;
using CsvHelper.Configuration;
using DatabaseConnector;
using DatabaseConnector.Connectors;
using Histograms;
using Histograms.Managers;
using PrintUtilities;
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
            PrintUtilitie.PrintLine($"Running Cleanup: {CleanupFile.Name}", 1, ConsoleColor.Red);
            await DatabaseModel.Connector.CallQuery(CleanupFile);

            PrintUtilitie.PrintLine($"Running Setup: {SetupFile.Name}", 1, ConsoleColor.Blue);
            await DatabaseModel.Connector.CallQuery(SetupFile);
            await HistogramManager.AddHistograms(SetupFile);

            Results = await RunQueriesSerial();

            PrintUtilitie.PrintLine($"Running Cleanup: {CleanupFile.Name}", 1, ConsoleColor.Red);
            await DatabaseModel.Connector.CallQuery(CleanupFile);

            if (consoleOutput)
                WriteResultToConsole();
            if (saveResult)
                SaveResult();


            return Results;
        }

        private async Task<List<TestCase>> RunQueriesSerial()
        {
            PrintUtilitie.PrintLine($"Running tests for [{DatabaseModel.Name}] connector", 2, ConsoleColor.Green);
            var testCases = new List<TestCase>();
            int count = 0;
            int max = CaseFiles.Count();
            foreach (FileInfo queryFile in CaseFiles)
            {
                try
                {
                    PrintUtilitie.PrintProgressBar(count, max, 50, true, 2);
                    PrintUtilitie.Print($"\t [File: {queryFile.Name}]    ", 0, ConsoleColor.Blue);
                    PrintUtilitie.Print($"\t Executing SQL statement...             ", 0);
                    DataSet dbResult = await DatabaseModel.Connector.AnalyseQuery(queryFile);
                    AnalysisResult analysisResult = DatabaseModel.Parser.ParsePlan(dbResult);
                    TestCase testCase = new TestCase(queryFile, analysisResult);
                    testCases.Add(testCase);
                }
                catch (Exception ex)
                {
                    PrintUtilitie.PrintLine($"Error! The query file [{queryFile}] failed with the following error:", 1);
                    PrintUtilitie.PrintLine(ex.ToString(), 1);
                    Console.WriteLine(ex);
                }
                count++;
            }
            PrintUtilitie.PrintProgressBar(50, 50, 50, true, 2);
            PrintUtilitie.PrintLine(" Finished!                                                             ", 0, ConsoleColor.Green);
            Console.WriteLine();
            return testCases;
        }

        private void WriteResultToConsole()
        {
            PrintUtilitie.PrintLine($"Displaying report for [{DatabaseModel.Name}] analysis", 2, ConsoleColor.Green);
            PrintUtilitie.PrintLine(FormatList("Category", "Case Name", "Predicted (Rows)", "Actual (Rows)"), 2, ConsoleColor.DarkGray);
            foreach (var testCase in Results)
                PrintUtilitie.PrintLine(FormatList(testCase.Category, testCase.Name, testCase.TestResult.EstimatedCardinality.ToString(), testCase.TestResult.ActualCardinality.ToString()), 2, ConsoleColor.Blue);
        }

        private string FormatList(string category, string caseName, string predicted, string actual)
        {
            return string.Format("{0,-30} {1,-20} {2,-20} {3,-20}", category, caseName, predicted, actual);
        }

        private void SaveResult()
        {
            csvWriter.Write();
        }
    }
}
