using ExperimentSuite.UserControls.SentinelReportViewer.Controls;
using ResultsSentinel;
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
using System.Windows.Threading;

namespace ExperimentSuite.UserControls.SentinelReportViewer
{
    /// <summary>
    /// Interaction logic for SentinelReportViewer.xaml
    /// </summary>
    public partial class SentinelReportViewer : Window
    {
        private List<IResultSentinel> _sentinels;

        public SentinelReportViewer(List<IResultSentinel> reportLines)
        {
            InitializeComponent();

            _sentinels = reportLines;

            foreach (IResultSentinel item in _sentinels)
                if (item.IsEnabled)
                    ReportPanel.Children.Add(new SentinelGrid(item.GetType().Name, item));
        }
    }
}
