using ExperimentSuite.Controllers;
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
        public delegate void StartRunEventHandler(TestRunner runner);
        public event StartRunEventHandler? RunnerStartedEvent;

        private RunnerController controller;

        public double CollapsedSize { get; } = 30;
        public double ExpandedSize { get; } = double.NaN;

        public TestRunner(string experimentName, string runName, string rootResultsPath, SuiteData runData, FileInfo settingsFile, FileInfo? setupFile, FileInfo? insertsFile, FileInfo? analyseFile, FileInfo? cleanupFile, IEnumerable<FileInfo> caseFiles)
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

            InitializeComponent();
            Toggle(true);

            TestNameLabel.Content = runName;
        }

        public async Task Run()
        {
            if (RunnerStartedEvent != null)
                RunnerStartedEvent.Invoke(this);
            await controller.Run();
        }

        private void UpdateHistogramProgressBar(double value, double max = 0)
        {
            if (max != 0)
                HistogramControl.HistogramProgressBar.Maximum = max;
            HistogramControl.HistogramProgressBar.Value = value;
        }

        private void UpdateSQLFileProgressBar(double value, double max = 0)
        {
            if (max != 0)
                SQLFileControl.SQLProgressBar.Maximum = max;
            SQLFileControl.SQLProgressBar.Value = value;
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