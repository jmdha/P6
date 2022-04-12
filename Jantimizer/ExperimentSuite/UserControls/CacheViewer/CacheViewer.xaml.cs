using Histograms.Caches;
using QueryPlanParser.Caches;
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
using Tools.Caches;

namespace ExperimentSuite.UserControls
{
    /// <summary>
    /// Interaction logic for CacheViewer.xaml
    /// </summary>
    public partial class CacheViewer : UserControl, ICollapsable
    {
        public double CollapsedSize { get; } = 0;
        public double ExpandedSize { get; }

        public CacheViewer()
        {
            InitializeComponent();
            ExpandedSize = MaxWidth;
            LoadCachesFromFile();
        }

        public void Toggle()
        {
            if (Width == CollapsedSize)
                Width = ExpandedSize;
            else
                Width = CollapsedSize;
        }

        public void Toggle(bool collapse)
        {
            if (collapse)
                Width = CollapsedSize;
            else
                Width = ExpandedSize;
        }

        private void ClearLocalCachesButton_Click(object sender, RoutedEventArgs e)
        {
            if (QueryPlanCacher.Instance != null)
                QueryPlanCacher.Instance.ClearCache();
            if (HistogramCacher.Instance != null)
                HistogramCacher.Instance.ClearCache();
            DataPanel.Children.Clear();
        }

        private void ClearFileCachesButton_Click(object sender, RoutedEventArgs e)
        {
            if (QueryPlanCacher.Instance != null)
                QueryPlanCacher.Instance.ClearCache(true);
            if (HistogramCacher.Instance != null)
                HistogramCacher.Instance.ClearCache(true);
            DataPanel.Children.Clear();
        }

        private void RefreshCachesButton_Click(object sender, RoutedEventArgs e)
        {
            DataPanel.Children.Clear();
            var cacheItems = new List<CacheItem>();
            if (HistogramCacher.Instance != null)
                cacheItems.AddRange(HistogramCacher.Instance.GetAllCacheItems());
            if (QueryPlanCacher.Instance != null)
                cacheItems.AddRange(QueryPlanCacher.Instance.GetAllCacheItems());

            CacheItemCountLabel.Content = $"{cacheItems.Count} item(s)";
            cacheItems.Insert(0, new CacheItem("Hash", "Data", "Cacher Service"));
            foreach (var cacheItem in cacheItems)
                DataPanel.Children.Add(new CacheItemControl(cacheItem));
        }

        private void LoadFromFile_Click(object sender, RoutedEventArgs e)
        {
            LoadCachesFromFile();
        }

        private void LoadCachesFromFile()
        {
            new QueryPlanCacher("query-plan-cache.json");
            new HistogramCacher("histogram-cache.json");
        }
    }
}
