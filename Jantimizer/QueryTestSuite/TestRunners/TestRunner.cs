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
        private Dictionary<PrinterCategory, PrintUtil> Printers = new Dictionary<PrinterCategory, PrintUtil>();

        public TestRunner(SuiteData runData, FileInfo settingsFile, FileInfo setupFile, FileInfo cleanupFile, IEnumerable<FileInfo> caseFiles, DateTime timeStamp, IConsole console)
        {
            RunData = runData;
            SettingsFile = settingsFile;
            SetupFile = setupFile;
            CleanupFile = cleanupFile;
            CaseFiles = caseFiles;
            Results = new List<TestCaseResult>();
            csvWriter = new CSVWriter($"Results/{timeStamp.ToString("yyyy/MM/dd/HH.mm.ss")}", "result.csv");
            Printers.Add(PrinterCategory.Primary, new PrintUtil(console));
            var consoles = console.SplitRows(
                new Split(10, "Status"),
                new Split(3, "Progress"),
                new Split(0, "Report")
            );
            Printers.Add(PrinterCategory.Status, new PrintUtil(consoles[0]));
            Printers.Add(PrinterCategory.Progress, new PrintUtil(consoles[1]));
            Printers.Add(PrinterCategory.Report, new PrintUtil(consoles[2]));
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
                int pbID = Printers[PrinterCategory.Progress].AddProgressBar(max);

                foreach (var t in tasks)
                {
                    t.Wait();
                    i++;
                    Printers[PrinterCategory.Progress].UpdateProgreesBar(pbID, i, $"Item {i} of {max}");
                }
            }

            if (RunData.Settings.DoRunTests != null && (bool)RunData.Settings.DoRunTests)
            {
                PrintTestUpdate("Begining Test Run for:", RunData.Name);
                Results = await RunQueriesSerial();
            }

            if (RunData.Settings.DoPostCleanup != null && (bool)RunData.Settings.DoPostCleanup)
            {
                PrintTestUpdate("Running Post-Cleanup", CleanupFile.Name, ConsoleColor.Red);
                await RunData.Connector.CallQuery(CleanupFile);
            }

            if (RunData.Settings.DoMakeReport != null && (bool)RunData.Settings.DoMakeReport)
            {
                PrintTestUpdate("Making Report", RunData.Name);
                if (consoleOutput)
                    WriteResultToConsole();
                if (saveResult)
                    SaveResult();
            }
            PrintTestUpdate("Tests finished for:", RunData.Name, ConsoleColor.Yellow);

            return Results;
        }

        private async Task<List<TestCaseResult>> RunQueriesSerial()
        {
            var testCases = new List<TestCaseResult>();
            int count = 1;
            int max = CaseFiles.Count();
            int pbID = Printers[PrinterCategory.Progress].AddProgressBar(max);
            foreach (var queryFile in CaseFiles)
            {
                try
                {
                    Printers[PrinterCategory.Progress].UpdateProgreesBar(pbID, count, queryFile.Name);
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

        private void WriteResultToConsole()
        {
            Printers[PrinterCategory.Report].PrintLine(new List<string>()
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

            foreach (var testCase in Results)
            {
                var DbAnalysisAccuracy = GetAccuracy(testCase.DbAnalysisResult.ActualCardinality, testCase.DbAnalysisResult.EstimatedCardinality);
                var JantimiserEstimateAccuracy = GetAccuracy(testCase.DbAnalysisResult.ActualCardinality, testCase.JantimiserResult.EstimatedCardinality);

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

                Printers[PrinterCategory.Report].PrintLine(new List<string>() {
                    testCase.Name,
                    testCase.DbAnalysisResult.EstimatedCardinality.ToString(),
                    testCase.JantimiserResult.EstimatedCardinality.ToString(),
                    testCase.DbAnalysisResult.ActualCardinality.ToString(),
                    GetAccuracyAsString(DbAnalysisAccuracy),
                    GetAccuracyAsString(JantimiserEstimateAccuracy)
                },
                GetFormatStrings(),
                colors,
                2);
            }
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
            Printers[PrinterCategory.Status].PrintLine(
                    new List<string>() { left, right },
                    new List<string>() { "{0,-30}", "{0,-30}" },
                    new List<ConsoleColor>() { leftColor, rightColor }, 1);
        }
    }
}
