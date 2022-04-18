using ExperimentSuite.Controllers;
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
        private CacheController controller;
        public double CollapsedSize { get; } = 0;
        public double ExpandedSize { get; }

        public CacheViewer()
        {
            controller = new CacheController();
            controller.ClearViewPanel += ClearDataPanel;

            InitializeComponent();
            ExpandedSize = MaxWidth;
            controller.LoadCachesFromFile();
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

        private void ClearDataPanel()
        {
            DataPanel.Children.Clear();
        }

        private void ClearLocalCachesButton_Click(object sender, RoutedEventArgs e)
        {
            controller.ClearLocalCaches();
        }

        private void ClearFileCachesButton_Click(object sender, RoutedEventArgs e)
        {
            controller.ClearFileAndLocalCaches();
        }

        private void RefreshCachesButton_Click(object sender, RoutedEventArgs e)
        {
            DataPanel.Children.Clear();
            var cacheItems = controller.GetAllCacheItems();

            CacheItemCountLabel.Content = $"{cacheItems.Count} item(s)";
            cacheItems.Insert(0, new CacheItem("Hash", "Data", "Cacher Service"));
            foreach (var cacheItem in cacheItems)
                DataPanel.Children.Add(new CacheItemControl(cacheItem));
        }

        private void LoadFromFile_Click(object sender, RoutedEventArgs e)
        {
            controller.LoadCachesFromFile();
        }
    }
}
