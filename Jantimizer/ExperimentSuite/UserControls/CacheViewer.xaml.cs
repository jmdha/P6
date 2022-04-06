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

namespace ExperimentSuite.UserControls
{
    /// <summary>
    /// Interaction logic for CacheViewer.xaml
    /// </summary>
    public partial class CacheViewer : UserControl, ICollapsable
    {
        public CacheViewer()
        {
            InitializeComponent();
            ExpandedSize = MaxWidth;
        }

        public double CollapsedSize { get; } = 0;

        public double ExpandedSize { get; }

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

        private void ClearCachesButton_Click(object sender, RoutedEventArgs e)
        {
            if (QueryPlanCacher.Instance != null)
                QueryPlanCacher.Instance.ClearCache();
            if (HistogramCacher.Instance != null)
                HistogramCacher.Instance.ClearCache();
        }
    }
}
