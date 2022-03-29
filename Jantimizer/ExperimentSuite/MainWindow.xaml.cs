using ExperimentSuite.Models;
using ExperimentSuite.Models.ExperimentParsing;
using ExperimentSuite.SuiteDatas;
using ExperimentSuite.UserControls;
using System;
using System.Collections.Generic;
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
using Tools.Helpers;

namespace ExperimentSuite
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Dictionary<string, List<Func<Task>>> ExecutionTasks;

        public MainWindow()
        {
            InitializeComponent();
            ExecutionTasks = new Dictionary<string, List<Func<Task>>>();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var pgData = SuiteDataSets.GetPostgresSD();
            var myData = SuiteDataSets.GetMySQLSD(pgData.QueryParserManager.QueryParsers[0]);
            var connectorSet = new List<SuiteData>() { pgData, myData };

            DateTime runTime = DateTime.UtcNow;

            var experimentsFile = IOHelper.GetFile("../../../experiments.json");
            var testsPath = IOHelper.GetDirectory("../../../Tests");
            var res = JsonSerializer.Deserialize(File.ReadAllText(experimentsFile.FullName), typeof(ExperimentList));
            if (res is ExperimentList expList) {
                foreach (var experiment in expList.Experiments)
                {
                    SetupPreData(experiment, connectorSet, testsPath, runTime);
                    await RunExperimentQueue(experiment.RunParallel);
                    SetupRunData(experiment, connectorSet, testsPath, runTime);
                    await RunExperimentQueue(experiment.RunParallel);
                }
            }
        }

        private void SetupRunData(ExperimentData experiment, List<SuiteData> connectorSet, DirectoryInfo bastTestPath, DateTime timestamp)
        {
            foreach (TestRunData data in experiment.RunData)
            {
                foreach (SuiteData suitData in connectorSet)
                {
                    if (data.ConnectorName == suitData.Name)
                    {
                        foreach (string testFile in data.TestFiles)
                        {
                            var newDir = IOHelper.GetDirectory(bastTestPath, testFile);

                            IOHelper.CreateDirIfNotExist(newDir.FullName, "Cases/");
                            var caseDir = IOHelper.GetDirectory(newDir.FullName, "Cases/");

                            TestRunner runner = new TestRunner(
                                testFile,
                                suitData,
                                IOHelper.GetFileVariant(newDir, "testSettings", suitData.Name.ToLower(), "json"),
                                IOHelper.GetFileVariantOrNone(newDir, "setup", suitData.Name.ToLower(), "sql"),
                                IOHelper.GetFileVariantOrNone(newDir, "cleanup", suitData.Name.ToLower(), "sql"),
                                IOHelper.GetInvariantsInDir(caseDir).Select(invariant => IOHelper.GetFileVariant(caseDir, invariant, suitData.Name.ToLower(), "sql")),
                                timestamp
                            );
                            TestsPanel.Children.Add(runner);

                            Func<Task> runFunc = async () => await runner.Run(true);

                            if (ExecutionTasks.ContainsKey(testFile))
                                ExecutionTasks[testFile].Add(runFunc);
                            else
                                ExecutionTasks.Add(testFile, new List<Func<Task>>() { runFunc });
                        }
                    }
                }
            }
        }

        private void SetupPreData(ExperimentData experiment, List<SuiteData> connectorSet, DirectoryInfo bastTestPath, DateTime timestamp)
        {
            foreach (TestRunData data in experiment.PreRunData)
            {
                foreach (SuiteData suitData in connectorSet)
                {
                    if (data.ConnectorName == suitData.Name)
                    {
                        foreach (string testFile in data.TestFiles)
                        {
                            var newDir = IOHelper.GetDirectory(bastTestPath, testFile);

                            IOHelper.CreateDirIfNotExist(newDir.FullName, "Cases/");
                            var caseDir = IOHelper.GetDirectory(newDir.FullName, "Cases/");

                            TestRunner runner = new TestRunner(
                                testFile,
                                suitData,
                                IOHelper.GetFileVariant(newDir, "testSettings", suitData.Name.ToLower(), "json"),
                                IOHelper.GetFileVariantOrNone(newDir, "setup", suitData.Name.ToLower(), "sql"),
                                IOHelper.GetFileVariantOrNone(newDir, "cleanup", suitData.Name.ToLower(), "sql"),
                                IOHelper.GetInvariantsInDir(caseDir).Select(invariant => IOHelper.GetFileVariant(caseDir, invariant, suitData.Name.ToLower(), "sql")),
                                timestamp
                            );
                            TestsPanel.Children.Add(runner);

                            Func<Task> runFunc = async () => await runner.Run(true);

                            if (ExecutionTasks.ContainsKey(testFile))
                                ExecutionTasks[testFile].Add(runFunc);
                            else
                                ExecutionTasks.Add(testFile, new List<Func<Task>>() { runFunc });
                        }
                    }
                }
            }
        }

        private async Task RunExperimentQueue(bool runParallel)
        {
            if (runParallel)
            {
                foreach (string key in ExecutionTasks.Keys)
                {
                    List<Task> results = new List<Task>();
                    foreach (Func<Task> funcs in ExecutionTasks[key])
                    {
                        results.Add(funcs.Invoke());
                    }
                    await Task.WhenAll(results.ToArray());
                }
            }
            else
                foreach (string key in ExecutionTasks.Keys)
                    foreach (Func<Task> funcs in ExecutionTasks[key])
                        await funcs.Invoke();
            ExecutionTasks.Clear();
        }
    }
}
