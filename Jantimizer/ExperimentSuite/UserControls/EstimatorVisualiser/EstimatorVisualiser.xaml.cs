using QueryEstimator;
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
using Tools.Models.JsonModels;

namespace ExperimentSuite.UserControls.EstimatorVisualiser
{
    /// <summary>
    /// Interaction logic for EstimatorVisualiser.xaml
    /// </summary>
    public partial class EstimatorVisualiser : Window
    {
        IQueryEstimator<JsonQuery> _estimator;
        public EstimatorVisualiser(IQueryEstimator<JsonQuery> estimator)
        {
            _estimator = estimator;
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
