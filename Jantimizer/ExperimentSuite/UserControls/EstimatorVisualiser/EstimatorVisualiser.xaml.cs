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
        private TreeViewItem _currentItem;
        private EstimatorResult _estimatorResult;
        public EstimatorVisualiser(EstimatorResult estimatorResult, string titleName)
        {
            _estimatorResult = estimatorResult;
            InitializeComponent();
            Title = titleName;
            _currentItem = new TreeViewItem();
            _currentItem.IsExpanded = true;
            ResultTreeView.Items.Add(_currentItem);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (_estimatorResult.ResultChain != null)
                TraverseTreeRec(_estimatorResult.ResultChain);
            int count = 1;
            var added = new List<string>();
            foreach (var bounds in _estimatorResult.TableAttributeBounds)
            {
                var newTreeItem = new TreeViewItem();
                newTreeItem.Header = $"Sweep {count}";
                BoundsView.Items.Add(newTreeItem);
                foreach (var bound in bounds.Value)
                {
                    if (!added.Contains($"{bound.Left}"))
                    {
                        added.Add($"{bound.Left}");
                        var newAttributeItem = new TreeViewItem();
                        newAttributeItem.IsExpanded = true;
                        newAttributeItem.Header = bound.Left.ToString();

                        var newLowerBoundItem = new TreeViewItem();
                        newLowerBoundItem.Background = Brushes.Red;
                        newLowerBoundItem.Header = $"{bound.MinLowerBound}...{bound.LowerBound - 1}";
                        var newUpperBoundItem = new TreeViewItem();
                        newUpperBoundItem.Background = Brushes.Red;
                        newUpperBoundItem.Header = $"{bound.UpperBound + 1}...{bound.MaxUpperBound}";

                        if (bound.LowerBound != bound.MinLowerBound)
                            newAttributeItem.Items.Add(newLowerBoundItem);
                        for (int i = bound.LowerBound; i <= bound.UpperBound; i++)
                        {
                            var newInnerTreeItem = new TreeViewItem();
                            newInnerTreeItem.Header = $"ID {i}";
                            newInnerTreeItem.Background = Brushes.Green;
                            newAttributeItem.Items.Add(newInnerTreeItem);
                        }
                        if (bound.UpperBound != bound.MaxUpperBound)
                            newAttributeItem.Items.Add(newUpperBoundItem);
                        newTreeItem.Items.Add(newAttributeItem);
                    }
                }
                added.Clear();
                count++;
            }
        }

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
