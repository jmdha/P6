using ExperimentSuite.Controllers;
using ExperimentSuite.Helpers;
using ExperimentSuite.UserControls.SentinelReportViewer;
using ResultsSentinel;
using ResultsSentinel.SentinelLog;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace ExperimentSuite
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DispatcherTimer dispatcherTimer = new DispatcherTimer();
        private ExperimentController controller;

        public MainWindow()
        {
            new OptimiserResultSentinel();
            new QueryPlanParserResultSentinel();
            new QueryParserResultSentinel();

            controller = new ExperimentController();

            controller.WriteToStatus += WriteToStatus;
            controller.UpdateExperimentProgressBar += UpdateExperimentProgressBar;
            controller.AddNewElement += AddNewElementToTestPanel;
            controller.RemoveElement += RemoveElementFromTestPanel;

            InitializeComponent();
            var iconHandle = Properties.Resources.icon;
            this.Icon = ImageHelper.ByteToImage(iconHandle);
            CacheViewerControl.Toggle(true);
        }

        private void UpdateExperimentProgressBar(double value, double max = 0)
        {
            if (max != 0)
                ExperimentProgressBar.Maximum = max;
            ExperimentProgressBar.Value = value;
        }

        private void AddNewElementToTestPanel(UIElement element, int index = -1)
        {
            if (index == -1)
                TestsPanel.Children.Add(element);
            else
                TestsPanel.Children.Insert(index, element);
        }

        private void RemoveElementFromTestPanel(UIElement element)
        {
            TestsPanel.Children.Remove(element);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //await controller.RunExperiments();
            RunButton.IsEnabled = true;
        }

        private void WriteToStatus(string text)
        {
            StatusTextbox.Text += $"{text}{Environment.NewLine}";
        }

        private void StatusTextbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox textBox)
                textBox.ScrollToEnd();
        }

        private async void RunButton_Click(object sender, RoutedEventArgs e)
        {
            RunButton.IsEnabled = false;
            StartSentinelTimerIfEnabled();
            await controller.RunExperiments();
            dispatcherTimer.Stop();
            RunButton.IsEnabled = true;
        }

        private void CacheViewerButton_Click(object sender, RoutedEventArgs e)
        {
            CacheViewerControl.Toggle();
        }

        private void OptimiserSentinelCheckbox_Click(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox checkBox)
                if (OptimiserResultSentinel.Instance != null)
                    OptimiserResultSentinel.Instance.IsEnabled = checkBox.IsEnabled;
        }

        private void QueryPlanSentinelCheckbox_Click(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox checkBox)
                if (QueryPlanParserResultSentinel.Instance != null)
                    QueryPlanParserResultSentinel.Instance.IsEnabled = checkBox.IsEnabled;
        }

        private void QuerySentinelCheckbox_Click(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox checkBox)
                if (QueryParserResultSentinel.Instance != null)
                    QueryParserResultSentinel.Instance.IsEnabled = checkBox.IsEnabled;
        }

        private void SentinelViewerButton_Click(object sender, RoutedEventArgs e)
        {
            var newList = new List<SentinelLogItem>();
            if (OptimiserResultSentinel.Instance != null)
                newList.AddRange(OptimiserResultSentinel.Instance.SentinelLog);
            if (QueryParserResultSentinel.Instance != null)
                newList.AddRange(QueryParserResultSentinel.Instance.SentinelLog);
            if (QueryPlanParserResultSentinel.Instance != null)
                newList.AddRange(QueryPlanParserResultSentinel.Instance.SentinelLog);
            var sentinelWindow = new SentinelReportViewer(newList);
            sentinelWindow.Show();
        }

        private void StartSentinelTimerIfEnabled()
        {
            bool any = false;
            if (!any && OptimiserResultSentinel.Instance != null)
                any = OptimiserResultSentinel.Instance.IsEnabled;
            if (!any && QueryParserResultSentinel.Instance != null)
                any = QueryParserResultSentinel.Instance.IsEnabled;
            if (!any && QueryPlanParserResultSentinel.Instance != null)
                any = QueryPlanParserResultSentinel.Instance.IsEnabled;

            if (any)
            {
                dispatcherTimer.Tick += dispatcherTimer_Tick;
                dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
                dispatcherTimer.Start();
            }
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            var count = 0;
            if (OptimiserResultSentinel.Instance != null)
                count += OptimiserResultSentinel.Instance.SentinelLog.Count;
            if (QueryParserResultSentinel.Instance != null)
                count += QueryParserResultSentinel.Instance.SentinelLog.Count;
            if (QueryPlanParserResultSentinel.Instance != null)
                count += QueryPlanParserResultSentinel.Instance.SentinelLog.Count;
            SentinelViewerButton.Content = $"Sentinels ({count})";
        }
    }
}
