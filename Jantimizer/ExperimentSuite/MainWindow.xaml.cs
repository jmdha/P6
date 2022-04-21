using ExperimentSuite.Controllers;
using ExperimentSuite.Helpers;
using ResultsSentinel;
using System;
using System.Windows;
using System.Windows.Controls;

namespace ExperimentSuite
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ExperimentController controller;

        public MainWindow()
        {
            new OptimiserResultSentinel();

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

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await controller.RunExperiments();
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
            await controller.RunExperiments();
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
    }
}
