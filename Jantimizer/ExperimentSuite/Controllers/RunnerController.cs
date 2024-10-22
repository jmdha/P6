﻿using ExperimentSuite.Helpers;
using ExperimentSuite.Models;
using ExperimentSuite.UserControls;
using Milestoner;
using QueryEstimator;
using QueryEstimator.Models;
using QueryPlanParser.Caches;
using QueryPlanParser.Models;
using ResultsSentinel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Tools.Helpers;
using Tools.Models.JsonModels;
using Tools.Services;

namespace ExperimentSuite.Controllers
{
    internal class RunnerController : IDisposable
    {
        public delegate void UpdateHistogramProgressBarHandler(double value, double max = 0);
        public event UpdateHistogramProgressBarHandler? UpdateHistogramProgressBar;

        public delegate void UpdateRunnerProgressBarHandler(double value, string fileName, string comment, string action, double max = 0);
        public event UpdateRunnerProgressBarHandler? UpdateRunnerProgressBar;

        public delegate void SetTestNameColorHandler(Brush brush);
        public event SetTestNameColorHandler? SetTestNameColor;

        public delegate void ToggleVisibilityHandler(bool state);
        public event ToggleVisibilityHandler? ToggleVisibility;

        public delegate void AddToReportPanelHandler(UIElement element);
        public event AddToReportPanelHandler? AddToReportPanel;

        public delegate void AddToTimeReportPanelHandler(UIElement element);
        public event AddToTimeReportPanelHandler? AddToTimeReportPanel;

        public delegate void AddToCaseTimeReportPanelHandler(UIElement element);
        public event AddToCaseTimeReportPanelHandler? AddToCaseTimeReportPanel;

        public delegate void PrintTestUpdateHandler(string left, string right);
        public event PrintTestUpdateHandler? PrintTestUpdate;

        public string ExperimentName { get; }
        public string RunnerName { get; }
        public SuiteData RunData { get; private set; }
        public FileInfo SettingsFile { get; private set; }
        public FileInfo? SetupFile { get; private set; }
        public FileInfo? DataInsertsFile { get; private set; }
        public FileInfo? DataAnalyseFile { get; private set; }
        public FileInfo? CleanupFile { get; private set; }
        public List<FileInfo> CaseFiles { get; private set; }
        public List<TestReport> Results { get; private set; }
        public List<TestTimeReport> TimeResults { get; private set; }
        public List<TestCaseTimeReport> CaseTimeResults { get; private set; }

        private CSVWriter csvWriter;
        private CSVWriter csvTimeWriter;
        private CSVWriter csvCaseTimeWriter;

        public RunnerController(string experimentName, string runnerName, string resultPath, SuiteData runData, FileInfo settingsFile, FileInfo? setupFile, FileInfo? dataInsertsFile, FileInfo? dataAnalyseFile, FileInfo? cleanupFile, List<FileInfo> caseFiles)
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
            TimeResults = new List<TestTimeReport>();
            CaseTimeResults = new List<TestCaseTimeReport>();
            csvWriter = new CSVWriter($"{resultPath}/Results/{experimentName}", $"{RunData.Name}-{RunnerName}.csv");
            csvTimeWriter = new CSVWriter($"{resultPath}/Times/{experimentName}", $"{RunData.Name}-{RunnerName}.csv");
            csvCaseTimeWriter = new CSVWriter($"{resultPath}/CaseTimes/{experimentName}", $"{RunData.Name}-{RunnerName}.csv");
        }

        public async Task<List<TestReport>> Run()
        {
            SetTestNameColor?.Invoke(Brushes.Yellow);
            ToggleVisibility?.Invoke(false);

            PrintTestUpdate?.Invoke("Parsing settings file:", SettingsFile.Name);
            var timer = TimerHelper.GetWatchAndStart();
            ParseTestSettings(SettingsFile);
            TimeResults.Add(timer.StopAndGetReportFromWatch(ExperimentName, RunData.Name, RunnerName, "Parse Test File"));

            if (IsTrueAndNotNull(RunData.Settings.DoPreCleanup))
            {
                if (CleanupFile == null)
                    throw new IOException("Cleanup file was null!");
                PrintTestUpdate?.Invoke("Running Pre-Cleanup", CleanupFile.Name);
                timer = TimerHelper.GetWatchAndStart();
                using (var connector = RunData.Connector)
                    await connector.CallQueryAsync(CleanupFile);
                TimeResults.Add(timer.StopAndGetReportFromWatch(ExperimentName, RunData.Name, RunnerName, "Pre Cleanup"));
            }

            if (IsTrueAndNotNull(RunData.Settings.DoSetup))
            {
                if (SetupFile == null)
                    throw new IOException("Setup file was null!");
                PrintTestUpdate?.Invoke("Running Setup", SetupFile.Name);
                timer = TimerHelper.GetWatchAndStart();
                using (var connector = RunData.Connector)
                    await connector.CallQueryAsync(SetupFile);
                TimeResults.Add(timer.StopAndGetReportFromWatch(ExperimentName, RunData.Name, RunnerName, "Setup"));
            }

            if (IsTrueAndNotNull(RunData.Settings.DoInserts))
            {
                if (DataInsertsFile == null)
                    throw new IOException("Inserts file was null!");
                PrintTestUpdate?.Invoke("Inserting Data", DataInsertsFile.Name);
                timer = TimerHelper.GetWatchAndStart();
                using (var connector = RunData.Connector)
                    await connector.CallQueryAsync(DataInsertsFile);
                TimeResults.Add(timer.StopAndGetReportFromWatch(ExperimentName, RunData.Name, RunnerName, "Inserts"));
            }

            if (IsTrueAndNotNull(RunData.Settings.DoAnalyse))
            {
                if (DataAnalyseFile == null)
                    throw new IOException("Analyse file was null!");
                PrintTestUpdate?.Invoke("Analysing Tables", DataAnalyseFile.Name);
                timer = TimerHelper.GetWatchAndStart();
                using (var connector = RunData.Connector)
                    await connector.CallQueryAsync(DataAnalyseFile);
                TimeResults.Add(timer.StopAndGetReportFromWatch(ExperimentName, RunData.Name, RunnerName, "Analyse"));
            }

            if (IsTrueAndNotNull(RunData.Settings.DoMakeHistograms))
            {
                PrintTestUpdate?.Invoke("Generating Histograms for:", RunData.Name);
                timer = TimerHelper.GetWatchAndStart();
                await GenerateMilestones(RunData.Milestoner);
                await BindMilestones(RunData.Milestoner);
                RunData.Milestoner.ClearDataCache();
                TimeResults.Add(timer.StopAndGetReportFromWatch(ExperimentName, RunData.Name, RunnerName, "Generate Histograms"));
            }

            if (IsTrueAndNotNull(RunData.Settings.DoRunTests))
            {
                PrintTestUpdate?.Invoke("Begining Test Run for:", RunData.Name);
                timer = TimerHelper.GetWatchAndStart();
                Results = await RunQueriesSerial();
                TimeResults.Add(timer.StopAndGetReportFromWatch(ExperimentName, RunData.Name, RunnerName, "Test run"));
            }

            if (IsTrueAndNotNull(RunData.Settings.DoPostCleanup))
            {
                if (CleanupFile == null)
                    throw new IOException("Cleanup file was null!");
                PrintTestUpdate?.Invoke("Running Post-Cleanup", CleanupFile.Name);
                timer = TimerHelper.GetWatchAndStart();
                using (var connector = RunData.Connector)
                    await connector.CallQueryAsync(CleanupFile);
                TimeResults.Add(timer.StopAndGetReportFromWatch(ExperimentName, RunData.Name, RunnerName, "Post Cleanup"));
            }

            if (IsTrueAndNotNull(RunData.Settings.DoMakeReport))
            {
                PrintTestUpdate?.Invoke("Making Report", RunData.Name);
                SaveResult();
            }

            if (IsTrueAndNotNull(RunData.Settings.DoMakeTimeReport))
            {
                PrintTestUpdate?.Invoke("Making Time Report", RunData.Name);
                var repMaker = new ReportMaker();
                repMaker.GenerateReport(Results);
                AddToReportPanel?.Invoke(repMaker);

                var timeRepMaker = new ReportMaker();
                timeRepMaker.GenerateReport(TimeResults);
                AddToTimeReportPanel?.Invoke(timeRepMaker);

                var caseTimeRepMaker = new ReportMaker();
                caseTimeRepMaker.GenerateReport(CaseTimeResults);
                AddToCaseTimeReportPanel?.Invoke(caseTimeRepMaker);
            }

            PrintTestUpdate?.Invoke("Tests finished for:", RunData.Name);

            ToggleVisibility?.Invoke(true);
            SetTestNameColor?.Invoke(Brushes.Green);

            return Results;
        }

        private async Task<List<TestReport>> RunQueriesSerial()
        {
            var testCases = new List<TestReport>();
            int max = CaseFiles.Count();
            int value = 0;
            UpdateRunnerProgressBar?.Invoke(value, "", "", "Starting...", max);
            foreach (var queryFile in CaseFiles)
            {
                while (SyncHelper.IsPaused)
                    await Task.Delay(100);

                using (var reader = new StreamReader(queryFile.FullName))
                {
                    var jsonQuery = new JsonQuery(await reader.ReadToEndAsync());
                    if (!jsonQuery.DoRun)
                        continue;

                    //RunData.Milestoner.UsedHistograms.Histograms.Clear();

                    // Get Cache
                    UpdateRunnerProgressBar?.Invoke(value++, queryFile.Name, jsonQuery.Comment, "Fetching cache...");
                    var timer = TimerHelper.GetWatchAndStart();
                    ulong? accCardinality = GetCacheIfThere(jsonQuery.EquivalentSQLQuery);
                    CaseTimeResults.Add(timer.StopAndGetCaseReportFromWatch(ExperimentName, RunData.Name, RunnerName, queryFile.Name, "Fetch cardinality cache"));

                    // Get DB result (with or without cache)
                    UpdateRunnerProgressBar?.Invoke(value, queryFile.Name, jsonQuery.Comment, "Executing query...");
                    timer = TimerHelper.GetWatchAndStart();
                    DataSet dbResult = await GetResultWithCache(jsonQuery.EquivalentSQLQuery, accCardinality != null);
                    CaseTimeResults.Add(timer.StopAndGetCaseReportFromWatch(ExperimentName, RunData.Name, RunnerName, queryFile.Name, "Get DB estimation"));

                    // Parse query plan
                    UpdateRunnerProgressBar?.Invoke(value, queryFile.Name, jsonQuery.Comment, "Parsing query plan...");
                    timer = TimerHelper.GetWatchAndStart();
                    AnalysisResult analysisResult = RunData.Parser.ParsePlan(dbResult);
                    if (QueryPlanParserResultSentinel.Instance != null)
                        QueryPlanParserResultSentinel.Instance.CheckResult(analysisResult, queryFile.Name, ExperimentName, RunnerName);
                    CaseTimeResults.Add(timer.StopAndGetCaseReportFromWatch(ExperimentName, RunData.Name, RunnerName, queryFile.Name, "Parse DB estimation"));

                    // Cache actual cardinalities (if not set)
                    UpdateRunnerProgressBar?.Invoke(value, queryFile.Name, jsonQuery.Comment, "Cache Result");
                    if (accCardinality == null)
                    {
                        timer = TimerHelper.GetWatchAndStart();
                        CacheCardinalities(analysisResult, jsonQuery.EquivalentSQLQuery);
                        CaseTimeResults.Add(timer.StopAndGetCaseReportFromWatch(ExperimentName, RunData.Name, RunnerName, queryFile.Name, "Cache cardinality"));
                    }
                    else
                        analysisResult.ActualCardinality = (ulong)accCardinality;

                    // Get Estimator prediction
                    UpdateRunnerProgressBar?.Invoke(value, queryFile.Name, jsonQuery.Comment, "Getting Estimator prediction...");
                    timer = TimerHelper.GetWatchAndStart();
                    EstimatorResult jantimatorResult = RunData.Estimator.GetQueryEstimation(jsonQuery);
                    CaseTimeResults.Add(timer.StopAndGetCaseReportFromWatch(ExperimentName, RunData.Name, RunnerName, queryFile.Name, "Estimator"));
                    if (EstimatorResultSentinel.Instance != null)
                        EstimatorResultSentinel.Instance.CheckResult(jantimatorResult, queryFile.Name, ExperimentName, RunnerName);

                    //if (HistogramResultSentinel.Instance != null)
                    //    HistogramResultSentinel.Instance.CheckResult(RunData.Milestoner.UsedHistograms, queryFile.Name, ExperimentName, RunnerName);

                    // Make test report
                    UpdateRunnerProgressBar?.Invoke(value, queryFile.Name, jsonQuery.Comment, "Generating Report...");
                    testCases.Add(
                        new TestReport(
                            ExperimentName,
                            RunnerName,
                            queryFile.Name,
                            RunData.Name,
                            analysisResult.EstimatedCardinality,
                            analysisResult.ActualCardinality,
                            jantimatorResult.EstimatedCardinality,
                            RunData.Milestoner.GetAbstractMilestoneStorageBytes(),
                            RunData.Milestoner.GetAbstractDatabaseStorageBytes(),
                            jantimatorResult)
                        );
                }
            }
            UpdateRunnerProgressBar?.Invoke(max, "", "", "Finished!");
            return testCases;
        }

        private async Task<DataSet> GetResultWithCache(string queryFileText, bool usingCache)
        {
            DataSet dbResult;
            using (var connector = RunData.Connector)
            {
                if (usingCache)
                    dbResult = await connector.ExplainQueryAsync(queryFileText);
                else
                    dbResult = await connector.AnalyseExplainQueryAsync(queryFileText);
            }
            return dbResult;
        }

        private void CacheCardinalities(AnalysisResult analysisResult, string queryFileText)
        {
            if (QueryPlanCacher.Instance != null)
                QueryPlanCacher.Instance.AddToCacheIfNotThere(new string[] { queryFileText, RunnerName }, analysisResult.ActualCardinality);
        }

        private ulong? GetCacheIfThere(string queryFileText)
        {
            ulong? accCardinality = null;
            if (QueryPlanCacher.Instance != null)
                accCardinality = QueryPlanCacher.Instance.GetValueOrNull(new string[] { queryFileText, RunnerName });
            return accCardinality;
        }

        private void SaveResult()
        {
            csvWriter.Write<TestReport, TestReportMap>(Results, true);
            csvTimeWriter.Write<TestTimeReport, TestTimeReportMap>(TimeResults, true);
            csvCaseTimeWriter.Write<TestCaseTimeReport, TestCaseTimeReportMap>(CaseTimeResults, true);
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

        public void Dispose()
        {
            UpdateHistogramProgressBar = null;
            UpdateRunnerProgressBar = null;
            SetTestNameColor = null;
            ToggleVisibility = null;
            AddToReportPanel = null;
            PrintTestUpdate = null;
            CaseFiles.Clear();
            SetupFile = null;
            DataInsertsFile = null;
            DataAnalyseFile = null;
            CleanupFile = null;
        }

        private async Task GenerateMilestones(IMilestoner milestoner)
        {
            milestoner.ClearMilestones();

            List<Func<Task>> milestoneTasks = await milestoner.AddMilestonesFromDBTasks();

            int value = 0;
            int max = milestoneTasks.Count;

            List<Task> results = new List<Task>();
            foreach (Func<Task> funcs in milestoneTasks)
            {
                results.Add(funcs.Invoke());
            }
            UpdateHistogramProgressBar?.Invoke(value, max);
            while (results.Any())
            {
                var finishedTask = await Task.WhenAny(results);
                results.Remove(finishedTask);
                await finishedTask;
                UpdateHistogramProgressBar?.Invoke(value++);
            }
            UpdateHistogramProgressBar?.Invoke(max);
        }

        private async Task BindMilestones(IMilestoner milestoner)
        {
            List<Func<Task>> compareTasks = milestoner.CompareMilestonesWithDBDataTasks();

            int value = 0;
            int max = compareTasks.Count;

            List<Task> results = new List<Task>();
            foreach (Func<Task> funcs in compareTasks)
            {
                results.Add(funcs.Invoke());
            }
            UpdateHistogramProgressBar?.Invoke(value, max);
            while (results.Any())
            {
                var finishedTask = await Task.WhenAny(results);
                results.Remove(finishedTask);
                await finishedTask;
                UpdateHistogramProgressBar?.Invoke(value++);
            }
            UpdateHistogramProgressBar?.Invoke(max);
        }
    }
}
