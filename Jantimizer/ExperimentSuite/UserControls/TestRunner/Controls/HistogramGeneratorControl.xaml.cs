using Histograms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace ExperimentSuite.UserControls
{
    /// <summary>
    /// Interaction logic for HistogramGeneratorControl.xaml
    /// </summary>
    public partial class HistogramGeneratorControl : UserControl
    {
        public HistogramGeneratorControl()
        {
            InitializeComponent();
        }

        public async Task GenerateHistograms(IHistogramManager manager)
        {
            List<Task> tasks = await manager.AddHistogramsFromDB();
            HistogramProgressBar.Maximum = tasks.Count;
            HistogramProgressBar.Value = 0;
            Update_HistogramProgressLabel(0, (int)HistogramProgressBar.Maximum);
            // https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/async/start-multiple-async-tasks-and-process-them-as-they-complete?pivots=dotnet-6-0#create-the-asynchronous-sum-page-sizes-method
            while (tasks.Any())
            {
                var finishedTask = await Task.WhenAny(tasks);
                tasks.Remove(finishedTask);
                await finishedTask;
                HistogramProgressBar.Value++;
                Update_HistogramProgressLabel((int)HistogramProgressBar.Value, (int)HistogramProgressBar.Maximum);
            }
            HistogramProgressBar.Value = HistogramProgressBar.Maximum;
            Update_HistogramProgressLabel((int)HistogramProgressBar.Maximum, (int)HistogramProgressBar.Maximum);
        }

        private void Update_HistogramProgressLabel(int current, int max)
        {
            HistogramProgressLabel.Content = $"Histogram Progress ({current}/{max})";
        }
    }
}
