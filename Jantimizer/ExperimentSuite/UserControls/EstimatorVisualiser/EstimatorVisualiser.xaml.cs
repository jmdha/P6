using QueryEstimator;
using QueryEstimator.Models;
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
        EstimatorResult _estimatorResult;
        public EstimatorVisualiser(EstimatorResult estimatorResult)
        {
            _estimatorResult = estimatorResult;
            InitializeComponent();
            _currentItem = new TreeViewItem();
            _currentItem.IsExpanded = true;
            ResultTreeView.Items.Add(_currentItem);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (_estimatorResult.ResultChain != null)
                TraverseTreeRec(_estimatorResult.ResultChain);
            foreach (var bounds in _estimatorResult.TableAttributeBounds)
            {
                var newLabel = new Label();
                newLabel.Content = "Sweep";
                BoundsPanel.Children.Add(newLabel);
                foreach(var bound in bounds.Value)
                    BoundsPanel.Children.Add(new TableAttributeBoundView(bound));
            }
        }

        private TreeViewItem _currentItem;
        private void TraverseTreeRec(ISegmentResult result)
        {
            if (result is SegmentResult seg)
            {
                var newItem = new TreeViewItem();
                newItem.IsExpanded = true;
                newItem.Header = $"Node Result: {seg.GetTotalEstimation()}";
                _currentItem.Items.Add(newItem);
                _currentItem = newItem;

                TraverseTreeRec(seg.Right);
                TraverseTreeRec(seg.Left);
            } 
            else if (result is ValueTableAttributeResult val)
            {
                var newItem = new TreeViewItem();
                newItem.IsExpanded = true;
                newItem.Header = $"{val.TableA} {ComparisonType.GetOperatorString(val.ComType)} {val.TableB} = {val.Count}";
                _currentItem.Items.Add(newItem);
            }
        }
    }
}
