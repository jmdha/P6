using ExperimentSuite.Helpers;
using ExperimentSuite.Models;
using ExperimentSuite.Models.ExperimentParsing;
using ExperimentSuite.SuiteDatas;
using ExperimentSuite.UserControls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Tools.Exceptions;
using Tools.Helpers;
using Tools.Services;

namespace ExperimentSuite.Controllers
{
    internal class ExperimentController
    {
        private readonly string ExperimentsFile = "../../../experiments.json";
        private readonly string TestsFolder = "../../../Tests";
        private readonly string ResultsFolder = "Results/";
        private readonly string ResultCSVFileName = "result.csv";
        private readonly string CaseFolderName = "Cases/";

        public delegate void WriteToStatusHandler(string text);
        public event WriteToStatusHandler? WriteToStatus;

        public delegate void AddTestRunnerHandler(UIElement element, int index = -1);
        public event AddTestRunnerHandler? AddNewElement;

        public delegate void RemoveTestRunnerHandler(UIElement element);
        public event RemoveTestRunnerHandler? RemoveElement;

        public delegate void UpdateProgressBarHandler(double value, double max = 0);
        public event UpdateProgressBarHandler? UpdateExperimentProgressBar;

        public ExperimentController()
        { 
        }

        public async Task RunExperiments()
        {
            try
            {
                string rootResultPath = $"{ResultsFolder}/{DateTime.UtcNow.ToString("yyyy-MM-dd HH.mm.ss")}";

                var testsPath = IOHelper.GetDirectory(TestsFolder);
                var expList = GetExperimentListFromFile();
                double max = expList.Experiments.Count;
                double value = 0;
                UpdateExperimentProgressBar?.Invoke(0, max);
                foreach (var experiment in expList.Experiments)
                {
                    if (experiment.RunExperiment)
                    {
                        UpdateExperimentProgressBar?.Invoke(value++);
                        AddNewElement?.Invoke(GetSeperatorLabel(experiment.ExperimentName),0);

                        // Find fitting suite data
                        WriteToStatus?.Invoke("Setting up suite datas...");
                        var connectorSet = SuiteDataSets.GetSuiteDatas(experiment.OptionalTestSettings);

                        // Setup labels
                        WriteToStatus?.Invoke($"Running experiment {experiment.ExperimentName}");
                        var awaitingLable = GetSeperatorLabel("Waiting...", 14);
                        AddNewElement?.Invoke(awaitingLable, 1);

                        // Pre run data
                        AddNewElement?.Invoke(GetSeperatorLabel("Setup", 14), 1);
                        var delDict = GetTestRunnerDelegatesFromTestFiles(
                            experiment.ExperimentName, 
                            experiment.PreRunData, 
                            connectorSet, 
                            testsPath, 
                            rootResultPath);
                        await TaskRunnerHelper.RunDelegates(delDict, experiment.RunParallel);
                        delDict.Clear();

                        // Run data
                        AddNewElement?.Invoke(GetSeperatorLabel("Tests", 14), 1);
                        delDict = GetTestRunnerDelegatesFromTestFiles(
                            experiment.ExperimentName,
                            experiment.RunData,
                            connectorSet,
                            testsPath,
                            rootResultPath);
                        await TaskRunnerHelper.RunDelegates(delDict, experiment.RunParallel);
                        delDict.Clear();
                        connectorSet.Clear();

                        GC.Collect();
                        GC.WaitForPendingFinalizers();

                        RemoveElement?.Invoke(awaitingLable);
                    }
                    WriteToStatus?.Invoke($"Experiment {experiment.ExperimentName} finished!");
                }
                UpdateExperimentProgressBar?.Invoke(max);
                WriteToStatus?.Invoke("All experiments complete!");
                SaveToCSV(rootResultPath);
            }
            catch (BaseErrorLogException ex)
            {
                var errorWindow = new ErrorLog();
                errorWindow.ErrorType.Content = ex.ActualException.GetType().Name;
                errorWindow.ErrorLabel.Content = ex.GetErrorLogMessage();
                errorWindow.ExceptionText.Content = ex.ActualException.Message;
                errorWindow.StackTraceTextbox.Text = ex.ActualException.StackTrace;
                errorWindow.Show();
            }
        }

        private Label GetSeperatorLabel(string text, int fontSize = 20)
        {
            var newLabel = new Label();
            newLabel.Content = text;
            newLabel.FontSize = fontSize;
            return newLabel;
        }

        private ExperimentList GetExperimentListFromFile()
        {
            WriteToStatus?.Invoke("Parsing experiment list...");
            var experimentsFile = IOHelper.GetFile(ExperimentsFile);
            var expList = JsonParsingHelper.ParseJson<ExperimentList>(File.ReadAllText(experimentsFile.FullName));
            return expList;
        }

        private void SaveToCSV(string path)
        {
            WriteToStatus?.Invoke("Merging results...");
            CSVMerger.Merge<TestReport, TestReportMap>(path, ResultCSVFileName);
            WriteToStatus?.Invoke("Merging finished");
        }

        private Dictionary<string, List<Func<Task>>> GetTestRunnerDelegatesFromTestFiles(string experimentName, List<TestRunData> runData, List<SuiteData> connectorSet, DirectoryInfo baseTestPath, string rootResultPath)
        {
            Dictionary<string, List<Func<Task>>> returnTasks = new Dictionary<string, List<Func<Task>>>();
            foreach (TestRunData data in runData)
            {
                foreach (SuiteData suitData in connectorSet)
                {
                    if (data.ConnectorName == suitData.Name && data.ConnectorID == suitData.ID)
                    {
                        foreach (string testFile in data.TestFiles)
                        {
                            var runFunc = CreateNewTestRunnerDelegate(testFile, experimentName, rootResultPath, suitData);

                            if (returnTasks.ContainsKey(testFile))
                                returnTasks[testFile].Add(runFunc);
                            else
                                returnTasks.Add(testFile, new List<Func<Task>>() { runFunc });
                        }
                    }
                }
            }
            return returnTasks;
        }

        private Func<Task> CreateNewTestRunnerDelegate(string testFile, string experimentName, string resultPath, SuiteData suiteData)
        {
            var newDir = IOHelper.GetDirectory(IOHelper.GetDirectory(TestsFolder), testFile);

            IOHelper.CreateDirIfNotExist(newDir.FullName, CaseFolderName);
            var caseDir = IOHelper.GetDirectory(newDir.FullName, CaseFolderName);

            TestRunner runner = new TestRunner(
                experimentName,
                testFile,
                resultPath,
                suiteData,
                IOHelper.GetFileVariant(newDir, "testSettings", suiteData.Name.ToLower(), "json"),
                IOHelper.GetFileVariantOrNull(newDir, "setup", suiteData.Name.ToLower(), "sql"),
                IOHelper.GetFileVariantOrNull(newDir, "inserts", suiteData.Name.ToLower(), "sql"),
                IOHelper.GetFileVariantOrNull(newDir, "analyse", suiteData.Name.ToLower(), "sql"),
                IOHelper.GetFileVariantOrNull(newDir, "cleanup", suiteData.Name.ToLower(), "sql"),
                IOHelper.GetInvariantsInDir(caseDir).Select(invariant => IOHelper.GetFileVariant(caseDir, invariant, suiteData.Name.ToLower(), "sql")).ToList()
            );
            runner.RunnerStartedEvent += RunnerStarted;
            AddNewElement?.Invoke(runner);

            Func<Task> runFunc = async () => await runner.Run();

            return runFunc;
        }

        private void RunnerStarted(TestRunner runner)
        {
            RemoveElement?.Invoke(runner);
            AddNewElement?.Invoke(runner,2);
        }
    }
}
