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
    /// Interaction logic for SQLFileProgressControl.xaml
    /// </summary>
    public partial class SQLFileProgressControl : UserControl
    {
        public string CurrentFileName { get; set; } = "";
        public string CurrentComment { get; set; } = "";
        public string CurrentAction { get; set; } = "";

        public SQLFileProgressControl()
        {
            InitializeComponent();
        }

        public void UpdateLabel()
        {
            SQLProgressLabel.Content = $"SQL Progress ({SQLProgressBar.Value}/{SQLProgressBar.Maximum}) [File: {CurrentFileName}], [Comment: {CurrentComment}] [Action: {CurrentAction}]";
        }
    }
}
