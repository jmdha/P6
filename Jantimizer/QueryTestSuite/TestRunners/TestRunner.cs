using CsvHelper;
using CsvHelper.Configuration;
using DatabaseConnector;
using DatabaseConnector.Connectors;
using Histograms;
using Histograms.Managers;
using Konsole;
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
        private enum PrinterCategory
        {
            None,
            Primary,
            Status,
            Progress,
            Report
        };

        public SuiteData RunData { get; }
        public FileInfo SettingsFile { get; private set; }
        public FileInfo SetupFile { get; private set; }
        public FileInfo CleanupFile { get; private set; }
        public IEnumerable<FileInfo> CaseFiles { get; private set; }
        public List<TestCaseResult> Results { get; private set; }
        private CSVWriter csvWriter;
        private WindowPrinter Printer;

        public TestRunner(SuiteData runData, FileInfo settingsFile, FileInfo setupFile, FileInfo cleanupFile, IEnumerable<FileInfo> caseFiles, DateTime timeStamp, WindowPrinter printer)
        {
            RunData = runData;
            SettingsFile = settingsFile;
            SetupFile = setupFile;
            CleanupFile = cleanupFile;
            CaseFiles = caseFiles;
            Results = new List<TestCaseResult>();
            csvWriter = new CSVWriter($"Results/{timeStamp.ToString("yyyy/MM/dd/HH.mm.ss")}", "result.csv");
            Printer = printer;
        }

        public async Task<List<TestCaseResult>> Run(bool consoleOutput = true, bool saveResult = true)
        {
            PrintTestUpdate("Parsing settings file:", SettingsFile.Name, ConsoleColor.Yellow);
            ParseTestSettings(SettingsFile);

            if (RunData.Settings.DoPreCleanup != null && (bool)RunData.Settings.DoPreCleanup)
            {
                PrintTestUpdate("Running Pre-Cleanup", CleanupFile.Name, ConsoleColor.Red);
                await RunData.Connector.CallQuery(CleanupFile);
            }

            if (RunData.Settings.DoSetup != null && (bool)RunData.Settings.DoSetup)
            {
                PrintTestUpdate("Running Setup", SetupFile.Name);
                await RunData.Connector.CallQuery(SetupFile);
            }

            if (RunData.Settings.DoMakeHistograms != null && (bool)RunData.Settings.DoMakeHistograms)
            {
                PrintTestUpdate("Generating Histograms for:", RunData.Name);
                List<Task> tasks = await RunData.HistoManager.AddHistogramsFromDB();
                int i = 0;
                int max = tasks.Count;
                int pbID = Printer.ProgressPrinter.AddProgressBar(max);

                foreach (var t in tasks)
                {
                    t.Wait();
                    i++;
                    Printer.ProgressPrinter.UpdateProgreesBar(pbID, i, $"Item {i} of {max}");
                }
            }

            if (consoleOutput && Printer.SplitWindow)
            {
                PrintTestUpdate("Writing Console Header For: ", RunData.Name);
                WriteHeaderToConsole();
            }
                

            if (RunData.Settings.DoRunTests != null && (bool)RunData.Settings.DoRunTests)
            {
                PrintTestUpdate("Begining Test Run for:", RunData.Name);
                Results = await RunQueriesSerial(true);
            }

            if (RunData.Settings.DoPostCleanup != null && (bool)RunData.Settings.DoPostCleanup)
            {
                PrintTestUpdate("Running Post-Cleanup", CleanupFile.Name, ConsoleColor.Red);
                await RunData.Connector.CallQuery(CleanupFile);
            }

            if (RunData.Settings.DoMakeReport != null && (bool)RunData.Settings.DoMakeReport)
            {
                if (consoleOutput)
                {
                    if (!Printer.SplitWindow)
                    {
                        PrintTestUpdate("Making report", RunData.Name);
                        WriteHeaderToConsole();
                        WriteResultToConsole();
                    }
                }
                if (saveResult)
                {
                    PrintTestUpdate("Saving result to file", RunData.Name);
                    SaveResult();
                }
                    
            }
            PrintTestUpdate("Tests finished for:", RunData.Name, ConsoleColor.Yellow);

            return Results;
        }

        private async Task<List<TestCaseResult>> RunQueriesSerial(bool consoleOutput = false)
        {
            var testCases = new List<TestCaseResult>();
            int count = 1;
            int max = CaseFiles.Count();
            int pbID = Printer.ProgressPrinter.AddProgressBar(max);
            foreach (var queryFile in CaseFiles)
            {
                try
                {
                    Printer.ProgressPrinter.UpdateProgreesBar(pbID, count, queryFile.Name);
                    DataSet dbResult = await RunData.Connector.AnalyseQuery(queryFile);
                    AnalysisResult analysisResult = RunData.Parser.ParsePlan(dbResult);

                    List<INode> nodes = RunData.QueryParserManager.ParseQuery(File.ReadAllText(queryFile.FullName), false);
                    AnalysisResult jantimiserResult = new AnalysisResult(
                        "Jantimiser",
                        0,
                        RunData.Optimiser.OptimiseQueryCardinality(nodes),
                        0,
                        new TimeSpan());

                    TestCaseResult testCase = new TestCaseResult(queryFile, analysisResult, jantimiserResult);
                    testCases.Add(testCase);
                    if (consoleOutput && Printer.SplitWindow)
                        WriteResultToConsole(testCase);
                }
                catch (Exception ex)
                {
                    //PrintUtil.PrintLine($"Error! The query file [{queryFile}] failed with the following error:", 1);
                    //PrintUtil.PrintLine(ex.ToString(), 1);
                }
                count++;
            }
            return testCases;
        }

        private void WriteHeaderToConsole()
        {
            Printer.GetReportPrinter(RunData.Name).PrintLine(new List<string>()
            {
                "Case Name",
                "P. Db Rows",
                "P. Jan Rows",
                "Actual Rows",
                "DB Acc  (%)",
                "Jan Acc (%)"
            },
            GetFormatStrings(),
            ConsoleColor.Blue,
            2);
        }

        private void WriteResultToConsole()
        {
            foreach (var result in Results)
                WriteResultToConsole(result);
        }

        private void WriteResultToConsole(TestCaseResult result)
        {
            var DbAnalysisAccuracy = GetAccuracy(result.DbAnalysisResult.ActualCardinality, result.DbAnalysisResult.EstimatedCardinality);
            var JantimiserEstimateAccuracy = GetAccuracy(result.DbAnalysisResult.ActualCardinality, result.JantimiserResult.EstimatedCardinality);

            var colors = new List<ConsoleColor>() {
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

            Printer.GetReportPrinter(RunData.Name).PrintLine(new List<string>() {
                    result.Name,
                    result.DbAnalysisResult.EstimatedCardinality.ToString(),
                    result.JantimiserResult.EstimatedCardinality.ToString(),
                    result.DbAnalysisResult.ActualCardinality.ToString(),
                    GetAccuracyAsString(DbAnalysisAccuracy),
                    GetAccuracyAsString(JantimiserEstimateAccuracy)
                },
            GetFormatStrings(),
            colors,
            2);
        }

        private string GetAccuracyAsString(decimal accuracy)
        {
            if (accuracy == 100)
                return "100      %";
            else if (accuracy == -1)
                return "inf      %";
            else
                return string.Format("{0, -8} %", accuracy);
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

        private List<string> GetFormatStrings()
        {
            return new List<string>() {
                    "{0, -18}",
                    "{0, -18}",
                    "{0, -18}",
                    "{0, -18}",
                    "{0, -15}",
                    "{0, -15}"
                };
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
                RunData.Settings.Update(set);
        }

        private void PrintTestUpdate(string left, string right, ConsoleColor leftColor = ConsoleColor.Blue, ConsoleColor rightColor = ConsoleColor.DarkGray)
        {
            Printer.StatusPrinter.PrintLine(
                    new List<string>() { left, right },
                    new List<string>() { "{0,-28}", "{0, 0}" },
                    new List<ConsoleColor>() { leftColor, rightColor }, 1);
        }
    }
}
