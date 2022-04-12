using ExperimentSuite.Models;
using ExperimentSuite.UserControls;
using Histograms;
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
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Tools.Helpers;
using Tools.Services;

namespace ExperimentSuite.Controllers
{
    internal class RunnerController
    {
        public delegate void UpdateHistogramProgressBarHandler(double value, double max = 0);
        public event UpdateHistogramProgressBarHandler? UpdateHistogramProgressBar;

        public delegate void UpdateRunnerProgressBarHandler(double value, double max = 0);
        public event UpdateRunnerProgressBarHandler? UpdateRunnerProgressBar;

        public delegate void SetTestNameColorHandler(Brush brush);
        public event SetTestNameColorHandler? SetTestNameColor;

        public delegate void ToggleVisibilityHandler(bool state);
        public event ToggleVisibilityHandler? ToggleVisibility;

        public delegate void AddToReportPanelHandler(UIElement element);
        public event AddToReportPanelHandler? AddToReportPanel;

        public delegate void PrintTestUpdateHandler(string left, string right);
        public event PrintTestUpdateHandler? PrintTestUpdate;

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


        private CSVWriter csvWriter;

        public RunnerController(string experimentName, string runnerName, string resultPath, SuiteData runData, FileInfo settingsFile, FileInfo? setupFile, FileInfo? dataInsertsFile, FileInfo? dataAnalyseFile, FileInfo? cleanupFile, IEnumerable<FileInfo> caseFiles)
        {
            ExperimentName = experimentName;
            RunnerName = runnerName;
            RunData = runData;
            SettingsFile = settingsFile;
            SetupFile = setupFile;
            DataInsertsFile = dataInsertsFile;
            DataAnalyseFile = dataAnalyseFile;
            CleanupFile = cleanupFile;
            CaseFiles = caseFiles;
            Results = new List<TestReport>();
            csvWriter = new CSVWriter($"{resultPath}/{experimentName}", $"{RunData.Name}-{RunnerName}.csv");
        }

        public async Task<List<TestReport>> Run()
        {
            SetTestNameColor?.Invoke(Brushes.Yellow);

            ToggleVisibility?.Invoke(false);

            PrintTestUpdate?.Invoke("Parsing settings file:", SettingsFile.Name);
            ParseTestSettings(SettingsFile);

            if (IsTrueAndNotNull(RunData.Settings.DoPreCleanup))
            {
                if (CleanupFile == null)
                    throw new IOException("Cleanup file was null!");
                PrintTestUpdate?.Invoke("Running Pre-Cleanup", CleanupFile.Name);
                await RunData.Connector.CallQueryAsync(CleanupFile);
            }

            if (IsTrueAndNotNull(RunData.Settings.DoSetup))
            {
                if (SetupFile == null)
                    throw new IOException("Setup file was null!");
                PrintTestUpdate?.Invoke("Running Setup", SetupFile.Name);
                await RunData.Connector.CallQueryAsync(SetupFile);
            }

            if (IsTrueAndNotNull(RunData.Settings.DoInserts))
            {
                if (DataInsertsFile == null)
                    throw new IOException("Inserts file was null!");
                PrintTestUpdate?.Invoke("Inserting Data", DataInsertsFile.Name);
                await RunData.Connector.CallQueryAsync(DataInsertsFile);
            }

            if (IsTrueAndNotNull(RunData.Settings.DoAnalyse))
            {
                if (DataAnalyseFile == null)
                    throw new IOException("Analyse file was null!");
                PrintTestUpdate?.Invoke("Analysing Tables", DataAnalyseFile.Name);
                await RunData.Connector.CallQueryAsync(DataAnalyseFile);
            }

            if (IsTrueAndNotNull(RunData.Settings.DoMakeHistograms))
            {
                PrintTestUpdate?.Invoke("Generating Histograms for:", RunData.Name);
                await GenerateHistograms(RunData.HistoManager);
            }

            if (IsTrueAndNotNull(RunData.Settings.DoRunTests))
            {
                PrintTestUpdate?.Invoke("Begining Test Run for:", RunData.Name);
                Results = await RunQueriesSerial();
            }

            if (IsTrueAndNotNull(RunData.Settings.DoPostCleanup))
            {
                if (CleanupFile == null)
                    throw new IOException("Cleanup file was null!");
                PrintTestUpdate?.Invoke("Running Post-Cleanup", CleanupFile.Name);
                await RunData.Connector.CallQueryAsync(CleanupFile);
            }

            if (IsTrueAndNotNull(RunData.Settings.DoMakeReport))
            {
                PrintTestUpdate?.Invoke("Making Report", RunData.Name);
                AddToReportPanel?.Invoke(new ReportMaker(Results));
                SaveResult();
            }

            PrintTestUpdate?.Invoke("Tests finished for:", RunData.Name);

            ToggleVisibility?.Invoke(true);
            SetTestNameColor?.Invoke(Brushes.Green);
            return Results;
        }

        private async Task<List<TestReport>> RunQueriesSerial()
        {
            var testCases = new List<TestReport>();
            int value = 0;
            UpdateRunnerProgressBar?.Invoke(value, CaseFiles.Count());
            foreach (var queryFile in CaseFiles)
            {
                UpdateRunnerProgressBar?.Invoke(value++);
                ulong? accCardinality = null;
                if (QueryPlanCacher.Instance != null)
                    accCardinality = QueryPlanCacher.Instance.GetValueOrNull(new string[] { File.ReadAllText(queryFile.FullName), RunnerName });
                DataSet dbResult = await GetResultWithCache(queryFile, accCardinality);

                AnalysisResult analysisResult = CacheActualCardinalitiesIfNotSet(dbResult, queryFile, accCardinality);

                List<INode> nodes = await RunData.QueryParserManager.ParseQueryAsync(File.ReadAllText(queryFile.FullName), false);
                OptimiserResult jantimiserResult = RunData.Optimiser.OptimiseQuery(nodes);

                TestReport testCase = new TestReport(ExperimentName, RunnerName, queryFile.Name, RunData.Name, analysisResult.EstimatedCardinality, analysisResult.ActualCardinality, jantimiserResult.EstTotalCardinality);
                testCases.Add(testCase);
            }
            UpdateRunnerProgressBar?.Invoke(CaseFiles.Count());
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

        private bool IsTrueAndNotNull(bool? value)
        {
            if (value != null)
                return (bool)value;
            return false;
        }

        private async Task GenerateHistograms(IHistogramManager manager)
        {
            List<Task> tasks = await manager.AddHistogramsFromDB();
            int value = 0;
            int max = tasks.Count;
            UpdateHistogramProgressBar?.Invoke(value, max);
            // https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/async/start-multiple-async-tasks-and-process-them-as-they-complete?pivots=dotnet-6-0#create-the-asynchronous-sum-page-sizes-method
            while (tasks.Any())
            {
                var finishedTask = await Task.WhenAny(tasks);
                tasks.Remove(finishedTask);
                await finishedTask;
                UpdateHistogramProgressBar?.Invoke(value++);
            }
            UpdateHistogramProgressBar?.Invoke(max);
        }
    }
}
