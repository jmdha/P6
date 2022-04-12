using DatabaseConnector.Exceptions;
using ExperimentSuite.Controllers;
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
        private ExperimentController controller;

        public MainWindow()
        {
            controller = new ExperimentController();

            controller.WriteToStatus += WriteToStatus;
            controller.SetCurrentExperimentLabel += SetCurrentExperimentLabel;
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

        private void SetCurrentExperimentLabel(string text)
        {
            ExperimentNameLabel.Content += $"{text}";
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
    }
}
