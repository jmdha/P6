using ExperimentSuite.Models;
using QueryOptimiser.Models;
using QueryParser.Models;
using QueryPlanParser.Caches;
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
using Tools.Caches;
using Tools.Helpers;
using Tools.Services;

namespace ExperimentSuite.UserControls
{
    /// <summary>
    /// Interaction logic for TestRunner.xaml
    /// </summary>
    public partial class TestRunner : UserControl, ICollapsable
    {
        public string ExperimentName { get; }
        public string RunnerName { get; }
        public SuiteData RunData { get; }
        public FileInfo SettingsFile { get; private set; }
        public FileInfo? SetupFile { get; private set; }
        public FileInfo? DataInsertsFile { get; private set; }
        public FileInfo? DataAnalyseFile { get; private set; }
        public FileInfo? CleanupFile { get; private set; }
        public IEnumerable<FileInfo> CaseFiles { get; private set; }
        public List<TestReport> Results { get; private set; }

        public double CollapsedSize { get; } = 30;

        public double ExpandedSize { get; } = double.NaN;

        private CSVWriter csvWriter;

        public TestRunner(string experimentName, string runName, string rootResultsPath, SuiteData runData, FileInfo settingsFile, FileInfo? setupFile, FileInfo? insertsFile, FileInfo? analyseFile, FileInfo? cleanupFile, IEnumerable<FileInfo> caseFiles)
        {
            ExperimentName = experimentName;
            RunnerName = runName;
            RunData = runData;
            SettingsFile = settingsFile;
            SetupFile = setupFile;
            DataInsertsFile = insertsFile;
            DataAnalyseFile = analyseFile;
            CleanupFile = cleanupFile;
            CaseFiles = caseFiles;
            Results = new List<TestReport>();
            csvWriter = new CSVWriter($"{rootResultsPath}/{experimentName}", $"{RunData.Name}-{RunnerName}.csv");
            InitializeComponent();
            Toggle(true);

            TestNameLabel.Content = RunnerName;
        }

        public async Task<List<TestReport>> Run(bool consoleOutput = true, bool saveResult = true)
        {
            TestNameLabel.Foreground = Brushes.Yellow;
            Toggle(false);

            PrintTestUpdate("Parsing settings file:", SettingsFile.Name);
            ParseTestSettings(SettingsFile);

            if (RunData.Settings.DoPreCleanup != null && (bool)RunData.Settings.DoPreCleanup)
            {
                if (CleanupFile == null)
                    throw new IOException("Cleanup file was null!");
                PrintTestUpdate("Running Pre-Cleanup", CleanupFile.Name);
                await RunData.Connector.CallQueryAsync(CleanupFile);
            }

            if (RunData.Settings.DoSetup != null && (bool)RunData.Settings.DoSetup)
            {
                if (SetupFile == null)
                    throw new IOException("Setup file was null!");
                PrintTestUpdate("Running Setup", SetupFile.Name);
                await RunData.Connector.CallQueryAsync(SetupFile);
            }

            if (RunData.Settings.DoInserts != null && (bool)RunData.Settings.DoInserts)
            {
                if (DataInsertsFile == null)
                    throw new IOException("Inserts file was null!");
                PrintTestUpdate("Inserting Data", DataInsertsFile.Name);
                await RunData.Connector.CallQueryAsync(DataInsertsFile);
            }

            if (RunData.Settings.DoAnalyse != null && (bool)RunData.Settings.DoAnalyse)
            {
                if (DataAnalyseFile == null)
                    throw new IOException("Analyse file was null!");
                PrintTestUpdate("Analysing Tables", DataAnalyseFile.Name);
                await RunData.Connector.CallQueryAsync(DataAnalyseFile);
            }

            if (RunData.Settings.DoMakeHistograms != null && (bool)RunData.Settings.DoMakeHistograms)
            {
                PrintTestUpdate("Generating Histograms for:", RunData.Name);
                await HistogramControl.GenerateHistograms(RunData.HistoManager);
            }

            if (RunData.Settings.DoRunTests != null && (bool)RunData.Settings.DoRunTests)
            {
                PrintTestUpdate("Begining Test Run for:", RunData.Name);
                Results = await RunQueriesSerial();
            }

            if (RunData.Settings.DoPostCleanup != null && (bool)RunData.Settings.DoPostCleanup)
            {
                if (CleanupFile == null)
                    throw new IOException("Cleanup file was null!");
                PrintTestUpdate("Running Post-Cleanup", CleanupFile.Name);
                await RunData.Connector.CallQueryAsync(CleanupFile);
            }

            if (RunData.Settings.DoMakeReport != null && (bool)RunData.Settings.DoMakeReport)
            {
                PrintTestUpdate("Making Report", RunData.Name);
                if (consoleOutput)
                    ReportPanel.Children.Add(new ReportMaker(Results));
                if (saveResult)
                    SaveResult();
            }

            PrintTestUpdate("Tests finished for:", RunData.Name);

            Toggle(true);
            TestNameLabel.Foreground = Brushes.Green;
            return Results;
        }

        private async Task<List<TestReport>> RunQueriesSerial()
        {
            var testCases = new List<TestReport>();
            SQLFileControl.SQLProgressBar.Maximum = CaseFiles.Count();
            foreach (var queryFile in CaseFiles)
            {
                await Task.Delay(1);
                SQLFileControl.UpdateFileLabel(queryFile.Name);
                SQLFileControl.SQLProgressBar.Value++;
                ulong? accCardinality = null;
                if (QueryPlanCacher.Instance != null)
                    accCardinality = QueryPlanCacher.Instance.GetValueOrNull(new string[] { File.ReadAllText(queryFile.FullName), RunnerName });
                DataSet dbResult = await GetResultWithCache(queryFile, accCardinality);

                AnalysisResult analysisResult = CacheActualCardinalitiesIfNotSet(dbResult, queryFile, accCardinality);

                List<INode> nodes = await RunData.QueryParserManager.ParseQueryAsync(File.ReadAllText(queryFile.FullName), false);
                nodes.Reverse();
                OptimiserResult jantimiserResult = RunData.Optimiser.OptimiseQuery(nodes);

                TestReport testCase = new TestReport(ExperimentName, RunnerName, queryFile.Name, RunData.Name, analysisResult.EstimatedCardinality, analysisResult.ActualCardinality, jantimiserResult.EstTotalCardinality);
                testCases.Add(testCase);
            }
            SQLFileControl.SQLProgressBar.Value = SQLFileControl.SQLProgressBar.Maximum;
            return testCases;
        }

        private async Task<DataSet> GetResultWithCache(FileInfo queryFile, ulong? accCardinality)
        {
            DataSet dbResult;
            if (accCardinality != null)
                dbResult = await RunData.Connector.ExplainQueryAsync(queryFile);
            else
                dbResult = await RunData.Connector.AnalyseExplainQueryAsync(queryFile);
            return dbResult;
        }

        private AnalysisResult CacheActualCardinalitiesIfNotSet(DataSet dbResult, FileInfo queryFile, ulong? accCardinality)
        {
            AnalysisResult analysisResult = RunData.Parser.ParsePlan(dbResult);
            if (accCardinality != null)
                analysisResult.ActualCardinality = (ulong)accCardinality;
            else
                if (QueryPlanCacher.Instance != null)
                    QueryPlanCacher.Instance.AddToCacheIfNotThere(new string[] { File.ReadAllText(queryFile.FullName), RunnerName }, analysisResult.ActualCardinality);
            return analysisResult;
        }

        private void SaveResult()
        {
            csvWriter.Write<TestReport, TestReportMap>(Results, true);
        }

        private void ParseTestSettings(FileInfo file)
        {
            if (!file.Exists)
                throw new IOException($"Error!, Test setting file `{file.Name}` not found!");
            RunData.Settings.Update(JsonParsingHelper.ParseJson<TestSettings>(File.ReadAllText(file.FullName)));
        }

        private void PrintTestUpdate(string left, string right)
        {
            StatusTextBox.Text += left + Environment.NewLine;
            FileStatusTextBox.Text += right + Environment.NewLine;
        }

        private void CollapseButton_Click(object sender, RoutedEventArgs e)
        {
            Toggle();
        }

        private void Textbox_Autoscroll_ToBottom(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox textBox)
                textBox.ScrollToEnd();
        }

        public void Toggle()
        {
            if (RunnerGrid.Height == CollapsedSize)
                RunnerGrid.Height = ExpandedSize;
            else
                RunnerGrid.Height = CollapsedSize;
        }

        public void Toggle(bool collapse)
        {
            if (collapse)
                RunnerGrid.Height = CollapsedSize;
            else
                RunnerGrid.Height = ExpandedSize;
        }
    }
}