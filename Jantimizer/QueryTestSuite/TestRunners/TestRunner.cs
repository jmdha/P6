using CsvHelper;
using CsvHelper.Configuration;
using DatabaseConnector;
using DatabaseConnector.Connectors;
using Histograms;
using Histograms.Managers;
using PrintUtilities;
using QueryOptimiser;
using QueryParser;
using QueryParser.Models;
using QueryParser.QueryParsers;
using QueryPlanParser.Models;
using QueryTestSuite.Models;
using QueryTestSuite.Services;
using System.Data;
using System.Text.Json;
using Tools.Models;

namespace QueryTestSuite.TestRunners
{
    internal class TestRunner
    {
        public SuiteData Case { get; }
        public FileInfo SettingsFile { get; private set; }
        public FileInfo SetupFile { get; private set; }
        public FileInfo CleanupFile { get; private set; }
        public IEnumerable<FileInfo> CaseFiles { get; private set; }
        public List<TestCaseResult> Results { get; private set; }
        private CSVWriter csvWriter;

        public TestRunner(SuiteData @case, FileInfo settingsFile, FileInfo setupFile, FileInfo cleanupFile, IEnumerable<FileInfo> caseFiles, DateTime timeStamp)
        {
            Case = @case;
            SettingsFile = settingsFile;
            SetupFile = setupFile;
            CleanupFile = cleanupFile;
            CaseFiles = caseFiles;
            Results = new List<TestCaseResult>();
            csvWriter = new CSVWriter($"Results/{timeStamp.ToString("yyyy/MM/dd/HH.mm.ss")}", "result.csv");
        }

        public async Task<List<TestCaseResult>> Run(bool consoleOutput = true, bool saveResult = true)
        {
            PrintUtil.PrintLine($"Parsing settings", 1, ConsoleColor.Blue);
            ParseTestSettings(SettingsFile);

            if (Case.Settings.DoPreCleanup != null && (bool)Case.Settings.DoPreCleanup)
            {
                PrintUtil.PrintLine($"Running Pre Cleanup: {CleanupFile.Name}", 1, ConsoleColor.Red);
                await Case.Connector.CallQuery(CleanupFile);
            }

            if (Case.Settings.DoSetup != null && (bool)Case.Settings.DoSetup)
            {
                PrintUtil.PrintLine($"Running Setup: {SetupFile.Name}", 1, ConsoleColor.Blue);
                await Case.Connector.CallQuery(SetupFile);
            }

            if (Case.Settings.DoMakeHistograms != null && (bool)Case.Settings.DoMakeHistograms)
            {
                PrintUtil.PrintLine($"Generating histograms", 1, ConsoleColor.Blue);
                await Case.HistoManager.AddHistogramsFromDB();
            }

            Results = await RunQueriesSerial();

            if (Case.Settings.DoPostCleanup != null && (bool)Case.Settings.DoPostCleanup)
            {
                PrintUtil.PrintLine($"Running Post Cleanup: {CleanupFile.Name}", 1, ConsoleColor.Red);
                await Case.Connector.CallQuery(CleanupFile);
            }

            if (consoleOutput)
                WriteResultToConsole();
            if (saveResult)
                SaveResult();

            return Results;
        }

        private async Task<List<TestCaseResult>> RunQueriesSerial()
        {
            PrintUtil.PrintLine($"Running tests for [{Case.Name}] connector", 2, ConsoleColor.Green);
            var testCases = new List<TestCaseResult>();
            int count = 0;
            int max = CaseFiles.Count();
            foreach (FileInfo queryFile in CaseFiles)
            {
                try
                {
                    PrintUtil.PrintProgressBar(count, max, 50, true, 2);
                    PrintUtil.Print($"\t [File: {queryFile.Name}]    ", 0, ConsoleColor.Blue);
                    PrintUtil.Print($"\t Executing SQL statement...             ", 0);
                    DataSet dbResult = await Case.Connector.AnalyseQuery(queryFile);
                    AnalysisResult analysisResult = Case.Parser.ParsePlan(dbResult);

                    List<INode> nodes = Case.QueryParserManager.ParseQuery(File.ReadAllText(queryFile.FullName), false);
                    AnalysisResult jantimiserResult = new AnalysisResult(
                        "Jantimiser",
                        0,
                        Case.Optimiser.OptimiseQueryCardinality(nodes),
                        0,
                        new TimeSpan());
                    
                    TestCaseResult testCase = new TestCaseResult(queryFile, analysisResult, jantimiserResult);
                    testCases.Add(testCase);
                }
                catch (Exception ex)
                {
                    PrintUtil.PrintLine($"Error! The query file [{queryFile}] failed with the following error:", 1);
                    PrintUtil.PrintLine(ex.ToString(), 1);
                }
                count++;
            }
            PrintUtil.PrintProgressBar(max, max, 50, true, 2);
            PrintUtil.PrintLine(" Finished!                                                             ", 0, ConsoleColor.Green);
            return testCases;
        }

        private void WriteResultToConsole()
        {
            PrintUtil.PrintLine($"Displaying report for [{Case.Name}] analysis", 2, ConsoleColor.Green);
            PrintUtil.PrintLine(FormatList("Category", "Case Name", "P. Db Rows", "P. Jantimiser Rows", "Actual Rows", "DB Acc (%)", "Jantimiser Acc (%)"), 2, ConsoleColor.DarkGray);

            foreach (var testCase in Results)
            {
                var DbAnalysisAccuracy = GetAccuracy(testCase.DbAnalysisResult.ActualCardinality, testCase.DbAnalysisResult.EstimatedCardinality);
                var JantimiserEstimateAccuracy = GetAccuracy(testCase.DbAnalysisResult.ActualCardinality, testCase.JantimiserResult.EstimatedCardinality);

                var colors = new List<ConsoleColor>() {
                    ConsoleColor.Blue,
                    ConsoleColor.Blue,
                    ConsoleColor.Blue,
                    ConsoleColor.Blue,
                    ConsoleColor.Blue,
                    ConsoleColor.Blue
                };

                if (DbAnalysisAccuracy > JantimiserEstimateAccuracy)
                    colors.Add(ConsoleColor.Red);
                else if (DbAnalysisAccuracy < JantimiserEstimateAccuracy)
                    colors.Add(ConsoleColor.Green);
                else
                    colors.Add(ConsoleColor.Yellow);

                PrintUtil.PrintLine(new List<string>() {
                    testCase.Category,
                    testCase.Name,
                    testCase.DbAnalysisResult.EstimatedCardinality.ToString(),
                    testCase.JantimiserResult.EstimatedCardinality.ToString(),
                    testCase.DbAnalysisResult.ActualCardinality.ToString(),
                    GetAccuracyAsString(DbAnalysisAccuracy),
                    GetAccuracyAsString(JantimiserEstimateAccuracy)
                }, 
                new List<string>() {
                    "{0, -30}",
                    "{0, -20}",
                    "{0, -20}", 
                    "{0, -20}", 
                    "{0, -20}", 
                    "{0, -10}", 
                    "{0, -10}"
                },
                colors,
                2);
            }
        }

        private string GetAccuracyAsString(decimal accuracy)
        {
            if (accuracy == 100)
                return "100   %";
            else if (accuracy == -1)
                return "inf   %";
            else 
                return string.Format("{0, -5} %", accuracy);
        }

        private decimal GetAccuracy(ulong actualValue, ulong predictedValue)
        {
            if (actualValue == 0 && predictedValue == 0)
                return 100;
            if (actualValue == 0)
                return -1;
            if (actualValue != 0 && predictedValue == 0)
                return -1;
            if (actualValue < predictedValue)
            {
                decimal value = ((decimal)actualValue / (decimal)predictedValue) * 100;
                return Math.Round(value, 2);
            }
            if (actualValue > predictedValue)
            {
                decimal value = ((decimal)predictedValue / (decimal)actualValue) * 100;
                return Math.Round(value, 2);
            }
            return 100;
        }

        private string FormatList(string category, string caseName, string predicted, string actual, string jantimiser, string dBAccuracy, string jantimiserAccuracy)
        {
            return string.Format("{0,-30} {1,-20} {2,-20} {3,-20} {4,-20} {5,-10} {6,-10}", category, caseName, predicted, actual, jantimiser, dBAccuracy, jantimiserAccuracy);
        }

        private void SaveResult()
        {
            csvWriter.Write<TestCaseResult, TestCaseResultMap>(Results, true);
        }

        private void ParseTestSettings(FileInfo file)
        {
            if (!file.Exists)
                throw new IOException($"Error!, Test setting file `{file.Name}` not found!");
            var res = JsonSerializer.Deserialize(File.ReadAllText(file.FullName), typeof(TestSettings));
            if (res is TestSettings set)
                Case.Settings.Update(set);
        }
    }
}
