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
        private List<SentinelLogItem> _logs;
        private IResultSentinel _sentinel;
        private string _sentinelName;
        private int tickCount = 10;
        private int maxTickCount = 10;
        private int currentItemCount = 0;
        private DispatcherTimer _sentinelCheckerTimer = new DispatcherTimer();
        public SentinelGrid(string sentinelName, IResultSentinel sentinel)
        {
            InitializeComponent();
            _sentinel = sentinel;
            _sentinelName = sentinelName;
            SentinelNameLabel.Content = sentinelName;
            _sentinelCheckerTimer.Tick += SentinelChecker_Tick;
            _sentinelCheckerTimer.Interval = new TimeSpan(0, 0, 1);
            _sentinelCheckerTimer.Start();
            _logs = new List<SentinelLogItem>();
        }

        private void SentinelChecker_Tick(object? sender, EventArgs e)
        {
            UpdateLabel(tickCount);
            tickCount--;
            if (tickCount < 0)
            {
                if (_sentinel.SentinelLog.Count != _logs.Count)
                {
                    _logs.Clear();
                    _logs.AddRange(_sentinel.SentinelLog);
                    currentItemCount = _logs.Count;
                    MainDataGrid.ItemsSource = null;
                    MainDataGrid.ItemsSource = _logs;
                }
                tickCount = maxTickCount;
            }
        }

        private void UpdateLabel(int count)
        {
            SentinelNameLabel.Content = $"{_sentinelName} (Updates in {count} seconds) (Total of {currentItemCount} log items)";
        }
    }
}
