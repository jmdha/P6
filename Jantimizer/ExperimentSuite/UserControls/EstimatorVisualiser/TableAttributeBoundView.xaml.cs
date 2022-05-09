using QueryEstimator.Models.BoundResults;
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

namespace ExperimentSuite.UserControls.EstimatorVisualiser
{
    /// <summary>
    /// Interaction logic for TableAttributeBoundView.xaml
    /// </summary>
    public partial class TableAttributeBoundView : UserControl
    {
        private IBoundResult _result;
        public TableAttributeBoundView(IBoundResult res)
        {
            _result = res;
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            AttrName.Content = _result.Left.ToString();
            for (int i = _result.MinLowerBound; i <= _result.MaxUpperBound; i++)
            {
                if (i < _result.LowerBound)
                {
                    var newCanvas = new Label();
                    newCanvas.Height = 30;
                    newCanvas.Background = Brushes.Red;
                    newCanvas.Content = $"ID: {i}";
                    MainPanel.Children.Add(newCanvas);
                } 
                else if (i > _result.UpperBound)
                {
                    var newCanvas = new Label();
                    newCanvas.Height = 30;
                    newCanvas.Background = Brushes.Red;
                    newCanvas.Content = $"ID: {i}";
                    MainPanel.Children.Add(newCanvas);
                }
                else
                {
                    var newCanvas = new Label();
                    newCanvas.Height = 30;
                    newCanvas.Background = Brushes.Green;
                    newCanvas.Content = $"ID: {i}";
                    MainPanel.Children.Add(newCanvas);
                }
            }
        }
    }
}
