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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace ExperimentSuite.UserControls.SentinelReportViewer.Controls
{
    /// <summary>
    /// Interaction logic for SentinelGrid.xaml
    /// </summary>
    public partial class SentinelGrid : UserControl
    {
        private IResultSentinel _sentinel;
        private string _sentinelName;
        private int tickCount = 10;
        private int maxTickCount = 10;
        private DispatcherTimer _sentinelCheckerTimer = new DispatcherTimer();
        public SentinelGrid(string sentinelName, IResultSentinel sentinel)
        {
            InitializeComponent();
            _sentinel = sentinel;
            _sentinelName = sentinelName;
            SentinelNameLabel.Content = sentinelName;
            _sentinelCheckerTimer.Tick += SentinelChecker_Tick;
            _sentinelCheckerTimer.Interval = new TimeSpan(0, 0, 1);
            MainDataGrid.ItemsSource = sentinel.SentinelLog;
            _sentinelCheckerTimer.Start();
        }

        private void SentinelChecker_Tick(object sender, EventArgs e)
        {
            UpdateLabel(tickCount);
            tickCount--;
            if (tickCount <= 0)
            {
                MainDataGrid.Items.Refresh();
                tickCount = maxTickCount;
            }
        }

        private void UpdateLabel(int count)
        {
            SentinelNameLabel.Content = $"{_sentinelName} (Updates in {count} seconds)";
        }
    }
}
