using ExperimentSuite.Models;
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
    /// Interaction logic for ReportMaker.xaml
    /// </summary>
    public partial class ReportMaker : UserControl
    {
        public ReportMaker(List<TestReport> reportLines = null)
        {
            InitializeComponent();
            if (reportLines != null)
                GenerateReport(reportLines);
        }

        public void GenerateReport(List<TestReport> reportLines)
        {
            MainGrid.ItemsSource = reportLines;
        }
    }
}
