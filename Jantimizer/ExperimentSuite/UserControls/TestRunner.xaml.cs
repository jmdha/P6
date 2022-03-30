using ExperimentSuite.Models;
using QueryOptimiser.Models;
using QueryParser.Models;
using QueryPlanParser.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Tools.Services;

namespace ExperimentSuite.UserControls
{
    /// <summary>
    /// Interaction logic for TestRunner.xaml
    /// </summary>
    public partial class TestRunner : UserControl
    {
        private int collapesedHeight = 40;

        public string RunnerName { get; }
        public SuiteData RunData { get; }
        public FileInfo SettingsFile { get; private set; }
        public FileInfo SetupFile { get; private set; }
        public FileInfo CleanupFile { get; private set; }
        public IEnumerable<FileInfo> CaseFiles { get; private set; }
        public List<TestCaseResult> Results { get; private set; }
        private CSVWriter csvWriter;

        public TestRunner(string name, SuiteData runData, FileInfo settingsFile, FileInfo setupFile, FileInfo cleanupFile, IEnumerable<FileInfo> caseFiles, DateTime timeStamp)
        {
            RunnerName = name;
            RunData = runData;
            SettingsFile = settingsFile;
            SetupFile = setupFile;
            CleanupFile = cleanupFile;
            CaseFiles = caseFiles;
            Results = new List<TestCaseResult>();
            csvWriter = new CSVWriter($"Results/{timeStamp.ToString("yyyy-MM-dd HH.mm.ss")}", $"{RunData.Name}-{RunnerName}.csv");
            InitializeComponent();

            RunnerGrid.Height = collapesedHeight;
            TestNameLabel.Content = RunnerName;
        }

        public async Task<List<TestCaseResult>> Run(bool consoleOutput = true, bool saveResult = true)
        {
            TestNameLabel.Foreground = Brushes.Yellow;
            RunnerGrid.Height = double.NaN;

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
                HistogramProgressBar.Maximum = tasks.Count;

                foreach (var t in tasks)
                {
                    await t;
                    HistogramProgressBar.Value++;
                }
                HistogramProgressBar.Value = HistogramProgressBar.Maximum;
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

            RunnerGrid.Height = collapesedHeight;
            TestNameLabel.Foreground = Brushes.Green;
            return Results;
        }

        private async Task<List<TestCaseResult>> RunQueriesSerial()
        {
            PrintUtilities.PrintUtil.PrintLine($"Running tests...", 2, ConsoleColor.Green);
            var testCases = new List<TestCaseResult>();
            int count = 0;
            SQLProgressBar.Maximum = CaseFiles.Count();
            foreach (var queryFile in CaseFiles)
            {
                try
                {
                    await Task.Delay(1);
                    CurrentSqlFileLabels.Content = $"File: {queryFile.Name}";
                    SQLProgressBar.Value++;
                    DataSet dbResult = await RunData.Connector.AnalyseQuery(queryFile);
                    AnalysisResult analysisResult = RunData.Parser.ParsePlan(dbResult);

                    List<INode> nodes = await RunData.QueryParserManager.ParseQueryAsync(File.ReadAllText(queryFile.FullName), false);
                    OptimiserResult jantimiserResult = RunData.Optimiser.OptimiseQuery(nodes);

                    TestCaseResult testCase = new TestCaseResult(RunData.Name, RunnerName, queryFile, RunData, analysisResult, jantimiserResult);
                    testCases.Add(testCase);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error! The query file [{queryFile}] failed with the following error: {ex.ToString()}");
                }
                count++;
            }
            SQLProgressBar.Value = SQLProgressBar.Maximum;
            return testCases;
        }

        private void WriteResultToConsole()
        {
            PrintTestUpdate("Displaying report...", RunData.Name);

            ReportTextBox.Text += PrintUtilities.FormatUtil.PrintLine(
                FormatList("Category", "Case Name", "P. Db Rows", "P. Jantimiser Rows", "Actual Rows", "DB Acc (%)", "Jantimiser Acc (%)"), 2);

            foreach (var testCase in Results)
            {
                var DbAnalysisAccuracy = GetAccuracy(testCase.DbAnalysisResult.ActualCardinality, testCase.DbAnalysisResult.EstimatedCardinality);
                var JantimiserEstimateAccuracy = GetAccuracy(testCase.DbAnalysisResult.ActualCardinality, testCase.JantimiserResult.EstTotalCardinality);

                ReportTextBox.Text += PrintUtilities.FormatUtil.PrintLine(new List<string>() {
                    testCase.Category,
                    testCase.Name,
                    testCase.DbAnalysisResult.EstimatedCardinality.ToString(),
                    testCase.JantimiserResult.EstTotalCardinality.ToString(),
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
                });
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
                RunData.Settings.Update(set);
        }

        private void PrintTestUpdate(string left, string right, ConsoleColor leftColor = ConsoleColor.Blue, ConsoleColor rightColor = ConsoleColor.DarkGray)
        {
            StatusTextBox.Text += PrintUtilities.FormatUtil.PrintLine(
                    new List<string>() { left, right },
                    new List<string>() { "{0,-30}", "{0,-30}" });
        }

        private void CollapseButton_Click(object sender, RoutedEventArgs e)
        {
            if (RunnerGrid.Height == collapesedHeight)
                RunnerGrid.Height = double.NaN;
            else
                RunnerGrid.Height = collapesedHeight;
        }

        private void StatusTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                textBox.ScrollToEnd();
            }
        }
    }
}
