using ExperimentSuite.Models;
using ExperimentSuite.Models.ExperimentParsing;
using ExperimentSuite.SuiteDatas;
using ExperimentSuite.UserControls;
using Histograms.Services;
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
using Tools.Helpers;
using Tools.Services;

namespace ExperimentSuite
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var iconHandle = ExperimentSuite.Properties.Resources.icon;
            this.Icon = ByteToImage(iconHandle);
        }

        public static ImageSource ByteToImage(byte[] imageData)
        {
            BitmapImage biImg = new BitmapImage();
            MemoryStream ms = new MemoryStream(imageData);
            biImg.BeginInit();
            biImg.StreamSource = ms;
            biImg.EndInit();

            ImageSource imgSrc = biImg as ImageSource;

            return imgSrc;
        }

        private async Task RunExperiments()
        {
            DateTime runTime = DateTime.UtcNow;

            new QueryPlanCacher();
            new HistogramCache();

            WriteToStatus("Parsing experiment list...");
            var experimentsFile = IOHelper.GetFile("../../../experiments.json");
            var testsPath = IOHelper.GetDirectory("../../../Tests");
            var expList = JsonParsingHelper.ParseJson<ExperimentList>(File.ReadAllText(experimentsFile.FullName));
            ExperimentProgressBar.Maximum = expList.Experiments.Count;
            ExperimentProgressBar.Value = 0;
            foreach (var experiment in expList.Experiments)
            {
                if (experiment.RunExperiment)
                {
                    ExperimentProgressBar.Value++;
                    ExperimentNameLabel.Content = experiment.ExperimentName;
                    TestsPanel.Children.Add(GetSeperatorLabel(experiment.ExperimentName));
                    var connectorSet = GetSuiteDatas(experiment.OptionalTestSettings);

                    WriteToStatus($"Running experiment {experiment.ExperimentName}");
                    await RunExperimentQueue(
                        GetRunDataFromList(experiment.PreRunData, connectorSet, testsPath, runTime),
                        experiment.RunParallel);
                    await RunExperimentQueue(
                        GetRunDataFromList(experiment.RunData, connectorSet, testsPath, runTime),
                        experiment.RunParallel);
                }
                WriteToStatus($"Experiment {experiment.ExperimentName} finished!");
            }
            ExperimentProgressBar.Value = ExperimentProgressBar.Maximum;
            WriteToStatus("All experiments complete!");
            RunButton.IsEnabled = true;
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

        private Dictionary<string, List<Func<Task>>> GetRunDataFromList(List<TestRunData> runData, List<SuiteData> connectorSet, DirectoryInfo bastTestPath, DateTime timestamp)
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

        private async Task RunExperimentQueue(Dictionary<string, List<Func<Task>>> dict, bool runParallel = true)
        {
            if (runParallel)
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
            else
                foreach (string key in dict.Keys)
                    foreach (Func<Task> funcs in dict[key])
                        await funcs.Invoke();
        }

        private void StatusTextbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                textBox.ScrollToEnd();
            }
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
    }
}
