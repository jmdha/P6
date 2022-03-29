using ExperimentSuite.Models.ExperimentParsing;
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
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var experimentsFile = IOHelper.GetFile("../../../experiments.json");
            var res = JsonSerializer.Deserialize(File.ReadAllText(experimentsFile.FullName), typeof(ExperimentList));
            if (res is ExperimentList expList) {
                foreach (var experiment in expList.Experiments)
                {

                }
            }
        }
    }
}
