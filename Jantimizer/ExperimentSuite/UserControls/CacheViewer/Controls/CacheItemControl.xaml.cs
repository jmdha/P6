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
    /// Interaction logic for CacheItemControl.xaml
    /// </summary>
    public partial class CacheItemControl : UserControl
    {
        public CacheItemControl(CacheItem item)
        {
            InitializeComponent();
            HashLable.Content = item.Hash;
            DataTextbox.Text = item.Content;
            CacherServiceLabel.Content = item.CacherServiceName;
        }
    }
}
