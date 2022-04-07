using Histograms.Caches;
using Microsoft.Win32;
using QueryPlanParser.Caches;
using System;
using System.Collections.Generic;
using System.IO;
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
using Tools.Caches;

namespace ExperimentSuite.UserControls
{
    /// <summary>
    /// Interaction logic for ErrorLog.xaml
    /// </summary>
    public partial class ErrorLog : Window
    {
        public ErrorLog()
        {
            InitializeComponent();
            PrintCacheToErrorLog();
        }

        public void PrintCacheToErrorLog()
        {
            CachePanel.Children.Clear();
            var cacheItems = new List<CacheItem>();
            if (HistogramCacher.Instance != null)
                cacheItems.AddRange(HistogramCacher.Instance.GetAllCacheItems());
            if (QueryPlanCacher.Instance != null)
                cacheItems.AddRange(QueryPlanCacher.Instance.GetAllCacheItems());

            cacheItems.Insert(0, new CacheItem("Hash", "Data", "Cacher Service"));
            foreach (var cacheItem in cacheItems)
                CachePanel.Children.Add(new CacheItemControl(cacheItem));
        }

        private void SaveToFile_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            if (saveFileDialog.ShowDialog() == true)
            {
                var sb = new StringBuilder();
                sb.AppendLine("Exception Name:");
                sb.AppendLine(ErrorType.Content.ToString());
                sb.AppendLine();

                sb.AppendLine("Error:");
                sb.AppendLine(ErrorLabel.Content.ToString());
                sb.AppendLine();

                sb.AppendLine("Exception Message:");
                sb.AppendLine(ExceptionText.Content.ToString());
                sb.AppendLine();

                sb.AppendLine("Stack Trace:");
                sb.AppendLine(StackTraceTextbox.Text);
                sb.AppendLine();

                sb.AppendLine("Cache Content:");
                foreach (var item in CachePanel.Children)
                {
                    if (item is CacheItemControl cacheItem)
                    {
                        sb.AppendLine("\t" + cacheItem.HashLable.Content.ToString());
                        sb.AppendLine("\t" + cacheItem.DataTextbox.Text);
                        sb.AppendLine("\t" + cacheItem.CacherServiceLabel.Content.ToString());
                        sb.AppendLine();
                    }
                }
                sb.AppendLine();

                File.WriteAllText(saveFileDialog.FileName, sb.ToString());
            }
        }
    }
}
