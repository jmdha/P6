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
        public ReportMaker()
        {
            InitializeComponent();
        }

        public void GenerateReport<T>(List<T> reportLines)
        {
            if (reportLines is List<TestReport> rep)
            {
                var newList = new List<TestReportView>();
                foreach(var report in rep)
                    newList.Add(new TestReportView(report));
                MainGrid.ItemsSource = newList;
            }
            else
                MainGrid.ItemsSource = reportLines;
        }
    }
}
