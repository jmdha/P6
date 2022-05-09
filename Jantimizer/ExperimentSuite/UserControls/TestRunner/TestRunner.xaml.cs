using ExperimentSuite.Controllers;
using ExperimentSuite.Models;
using ExperimentSuite.UserControls.MilestoneVisualiser;
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
        public delegate void StartRunEventHandler(TestRunner runner);
        public event StartRunEventHandler? RunnerStartedEvent;

        private RunnerController controller;

        public double CollapsedSize { get; } = 30;
        public double ExpandedSize { get; } = double.NaN;

        public TestRunner(string experimentName, string runName, string rootResultsPath, SuiteData runData, FileInfo settingsFile, FileInfo? setupFile, FileInfo? insertsFile, FileInfo? analyseFile, FileInfo? cleanupFile, List<FileInfo> caseFiles)
        {
            controller = new RunnerController(
                experimentName,
                runName,
                rootResultsPath,
                runData,
                settingsFile,
                setupFile,
                insertsFile,
                analyseFile,
                cleanupFile,
                caseFiles);

            controller.PrintTestUpdate += PrintTestUpdate;
            controller.UpdateRunnerProgressBar += UpdateSQLFileProgressBar;
            controller.UpdateHistogramProgressBar += UpdateHistogramProgressBar;
            controller.SetTestNameColor += SetTestNameColor;
            controller.ToggleVisibility += Toggle;
            controller.AddToReportPanel += AddToReportPanel;
            controller.AddToTimeReportPanel += AddToTimeReportPanel;
            controller.AddToCaseTimeReportPanel += AddToCaseTimeReportPanel;

            InitializeComponent();
            Toggle(true);

            TestNameLabel.Content = runName;
        }

        public async Task Run()
        {
            if (RunnerStartedEvent != null)
                RunnerStartedEvent.Invoke(this);
            await controller.Run();

            if (controller.Results.Count > 0)
            {
                decimal totalEstimatorAcc = 0;
                foreach (var result in controller.Results)
                    totalEstimatorAcc += result.EstimatorAcc;
                totalEstimatorAcc /= controller.Results.Count;

                UpdateLabelWithColor(TotalEstimatorAccuracyLabel, totalEstimatorAcc, "Estimator total accuracy:");

                decimal totalDatabaseAcc = 0;
                foreach (var result in controller.Results)
                    totalDatabaseAcc += result.DatabaseAcc;
                totalDatabaseAcc /= controller.Results.Count;

                UpdateLabelWithColor(TotalDatabaseAccuracyLabel, totalDatabaseAcc, "Database total accuracy:");

                ShowEstimatorResultCombobox.Items.Clear();
                foreach (var res in controller.Results)
                    ShowEstimatorResultCombobox.Items.Add(res.CaseName);
            }

            ShowMilestonesButton.IsEnabled = true;
            ShowEstimatorResultCombobox.IsEnabled = true;
        }

        private void UpdateLabelWithColor(Label control, decimal totalAcc, string additionalText)
        {
            if (totalAcc < 15)
                control.Foreground = Brushes.Red;
            else if (totalAcc < 30)
                control.Foreground = Brushes.DarkOrange;
            else if (totalAcc < 50)
                control.Foreground = Brushes.Orange;
            else if (totalAcc < 80)
                control.Foreground = Brushes.Yellow;
            else if (totalAcc < 90)
                control.Foreground = Brushes.DarkGreen;
            else
                control.Foreground = Brushes.Green;

            control.Content = $"{additionalText} {Math.Round(totalAcc, 2)}%";
        }

        private void UpdateHistogramProgressBar(double value, double max = 0)
        {
            if (max != 0)
                HistogramControl.HistogramProgressBar.Maximum = max;
            HistogramControl.HistogramProgressBar.Value = value;
        }

        private void UpdateSQLFileProgressBar(double value, string fileName, string comment, string action, double max = 0)
        {
            if (max != 0)
                SQLFileControl.SQLProgressBar.Maximum = max;
            SQLFileControl.SQLProgressBar.Value = value;
            SQLFileControl.CurrentFileName = fileName;
            SQLFileControl.CurrentComment = comment;
            SQLFileControl.CurrentAction = action;
            SQLFileControl.UpdateLabel();
        }

        private void PrintTestUpdate(string left, string right)
        {
            StatusTextBox.Text += left + Environment.NewLine;
            FileStatusTextBox.Text += right + Environment.NewLine;
        }

        private void SetTestNameColor(Brush brush)
        {
            TestNameLabel.Foreground = brush;
        }

        private void AddToReportPanel(UIElement element)
        {
            ReportPanel.Children.Add(element);
        }

        private void AddToTimeReportPanel(UIElement element)
        {
            TimeReportPanel.Children.Add(element);
        }

        private void AddToCaseTimeReportPanel(UIElement element)
        {
            CaseTimeReportPanel.Children.Add(element);
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

        private void ShowMilestonesButton_Click(object sender, RoutedEventArgs e)
        {
            var milestoneView = new MilestoneVisualiser.MilestoneVisualiser(controller.RunData.Milestoner, $"Milestone Data for '{controller.RunnerName}'");
            milestoneView.Show();
        }

        private void ShowEstimatorResultCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox box)
            {
                foreach(var result in controller.Results)
                {
                    if (box.SelectedItem != null && result.CaseName == box.SelectedItem.ToString() && result.EstimatorResult != null)
                    {
                        var estimatorView = new EstimatorVisualiser.EstimatorVisualiser(result.EstimatorResult, $"Estimator Result for '{result.CaseName}'");
                        estimatorView.Show();
                        break;
                    }
                }
            }
        }
    }
}