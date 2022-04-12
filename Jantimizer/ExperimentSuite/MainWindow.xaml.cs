using DatabaseConnector.Exceptions;
using ExperimentSuite.Helpers;
using ExperimentSuite.Models;
using ExperimentSuite.Models.ExperimentParsing;
using ExperimentSuite.SuiteDatas;
using ExperimentSuite.UserControls;
using Histograms.Caches;
using QueryPlanParser.Caches;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
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
using Tools.Exceptions;
using Tools.Helpers;
using Tools.Services;

namespace ExperimentSuite
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly string ExperimentsFile = "../../../experiments.json";
        private readonly string TestsFolder = "../../../Tests";
        private readonly string ResultsFolder = "Results/";
        private readonly string CaseFolderName = "Cases/";

        public MainWindow()
        {
            InitializeComponent();
            var iconHandle = Properties.Resources.icon;
            this.Icon = ImageHelper.ByteToImage(iconHandle);
            CacheViewerControl.Toggle(true);
        }

        private async Task RunExperiments()
        {
            try
            {
                string rootResultPath = $"{ResultsFolder}/{DateTime.UtcNow.ToString("yyyy-MM-dd HH.mm.ss")}";

                var testsPath = IOHelper.GetDirectory(TestsFolder);
                var expList = GetExperimentListFromFile();
                UpdateExperimentProgressBar(0, expList.Experiments.Count);
                foreach (var experiment in expList.Experiments)
                {
                    if (experiment.RunExperiment)
                    {
                        UpdateExperimentProgressBar(ExperimentProgressBar.Value + 1);
                        ExperimentNameLabel.Content = experiment.ExperimentName;
                        TestsPanel.Children.Add(GetSeperatorLabel(experiment.ExperimentName));

                        var connectorSet = GetSuiteDatas(experiment.OptionalTestSettings);
                        WriteToStatus($"Running experiment {experiment.ExperimentName}");
                        await RunExperimentQueue(
                            GetTestRunnerDelegatesFromTestFiles(experiment.ExperimentName, experiment.PreRunData, connectorSet, testsPath, rootResultPath),
                            experiment.RunParallel);
                        await RunExperimentQueue(
                            GetTestRunnerDelegatesFromTestFiles(experiment.ExperimentName, experiment.RunData, connectorSet, testsPath, rootResultPath),
                            experiment.RunParallel);
                    }
                    WriteToStatus($"Experiment {experiment.ExperimentName} finished!");
                }
                UpdateExperimentProgressBar(ExperimentProgressBar.Maximum);
                WriteToStatus("All experiments complete!");
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
            RunButton.IsEnabled = true;
        }

        private void UpdateExperimentProgressBar(double value, double max = 0)
        {
            if (max != 0)
                ExperimentProgressBar.Maximum = max;
            ExperimentProgressBar.Value = value;
        }

        private ExperimentList GetExperimentListFromFile()
        {
            WriteToStatus("Parsing experiment list...");
            var experimentsFile = IOHelper.GetFile(ExperimentsFile);
            var expList = JsonParsingHelper.ParseJson<ExperimentList>(File.ReadAllText(experimentsFile.FullName));
            return expList;
        }

        private void SaveToCSV(string path)
        {
            WriteToStatus("Merging results");
            CSVMerger.Merge<TestReport, TestReportMap>(path, "results.csv");
            WriteToStatus("Merging finished");
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await RunExperiments();
        }

        private void WriteToStatus(string text)
        {
            StatusTextbox.Text += $"{text}{Environment.NewLine}";
        }

        private List<SuiteData> GetSuiteDatas(JsonObject optionalSettings)
        {
            WriteToStatus("Setting up suite datas...");

            var pgDataDefault = SuiteDataSets.GetPostgresSD_Default(optionalSettings);
            var pgDataEquiDepth = SuiteDataSets.GetPostgresSD_EquiDepth(optionalSettings);
            var pgDataEquiDepthVariance = SuiteDataSets.GetPostgresSD_EquiDepthVariance(optionalSettings);

            var myDataDefault = SuiteDataSets.GetMySQLSD_Default(optionalSettings, pgDataDefault.QueryParserManager.QueryParsers[0]);
            var myDataEquiDepth = SuiteDataSets.GetMySQLSD_EquiDepth(optionalSettings, pgDataEquiDepth.QueryParserManager.QueryParsers[0]);
            var myDataEquiDepthVariance = SuiteDataSets.GetMySQLSD_EquiDepthVariance(optionalSettings, pgDataEquiDepthVariance.QueryParserManager.QueryParsers[0]);

            var connectorSet = new List<SuiteData>() { pgDataDefault, myDataDefault, pgDataEquiDepth, myDataEquiDepth, pgDataEquiDepthVariance, myDataEquiDepthVariance };
            return connectorSet;
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
                IOHelper.GetInvariantsInDir(caseDir).Select(invariant => IOHelper.GetFileVariant(caseDir, invariant, suiteData.Name.ToLower(), "sql"))
            );
            TestsPanel.Children.Add(runner);

            Func<Task> runFunc = async () => await runner.Run(true);

            return runFunc;
        }

        private async Task RunExperimentQueue(Dictionary<string, List<Func<Task>>> dict, bool runParallel = true)
        {
            if (runParallel)
                await RunExperimentQueueParallel(dict);
            else
                await RunExperimentQueueSerial(dict);
        }

        private async Task RunExperimentQueueParallel(Dictionary<string, List<Func<Task>>> dict)
        {
            foreach (string key in dict.Keys)
            {
                List<Task> results = new List<Task>();
                foreach (Func<Task> funcs in dict[key])
                {
                    results.Add(funcs.Invoke());
                }
                await Task.WhenAll(results.ToArray());
            }
        }

        private async Task RunExperimentQueueSerial(Dictionary<string, List<Func<Task>>> dict)
        {
            foreach (string key in dict.Keys)
                foreach (Func<Task> funcs in dict[key])
                    await funcs.Invoke();
        }

        private void StatusTextbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox textBox)
                textBox.ScrollToEnd();
        }

        private Label GetSeperatorLabel(string text)
        {
            var newLabel = new Label();
            newLabel.Content = text;
            newLabel.FontSize = 20;
            return newLabel;
        }

        private async void RunButton_Click(object sender, RoutedEventArgs e)
        {
            RunButton.IsEnabled = false;
            await RunExperiments();
        }

        private void CacheViewerButton_Click(object sender, RoutedEventArgs e)
        {
            CacheViewerControl.Toggle();
        }
    }
}
