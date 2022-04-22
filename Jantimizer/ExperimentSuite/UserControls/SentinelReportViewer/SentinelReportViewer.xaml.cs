using ResultsSentinel.SentinelLog;
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
using System.Windows.Shapes;

namespace ExperimentSuite.UserControls.SentinelReportViewer
{
    /// <summary>
    /// Interaction logic for SentinelReportViewer.xaml
    /// </summary>
    public partial class SentinelReportViewer : Window
    {
        public SentinelReportViewer(List<SentinelLogItem> reportLines)
        {
            InitializeComponent();
            MainGrid.ItemsSource = reportLines;
        }
    }
}
