using Milestoner;
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
using Tools.Models.JsonModels;

namespace ExperimentSuite.UserControls.MilestoneVisualiser.Controls
{
    /// <summary>
    /// Interaction logic for TableAttributeDataView.xaml
    /// </summary>
    public partial class TableAttributeDataView : UserControl
    {
        private IMilestoner _milestoner;
        private TableAttribute _attr;

        public TableAttributeDataView(IMilestoner milestoner, TableAttribute tableAttribute)
        {
            _milestoner = milestoner;
            _attr = tableAttribute;
            InitializeComponent();

            DataForLabel.Content = $"Data for [{_attr}]";
            var allMilestones = _milestoner.GetMilestonesNoAlias(_attr);
            long totalRowCount = 0;
            foreach (var milestone in allMilestones)
                totalRowCount += milestone.ElementsBeforeNextSegmentation;
            RowCountLabel.Content = $"There are {totalRowCount} rows for this attribute";
            TotalMilestoneCountLabel.Content = $"There are {allMilestones.Count} milestones for this attribute";
        }

        private void LoadMilestonesButton_Click(object sender, RoutedEventArgs e)
        {
            MilestonePanel.Children.Clear();
            foreach (var milestone in _milestoner.GetMilestonesNoAlias(_attr))
                MilestonePanel.Children.Add(new MilestoneViewer(milestone));
        }
    }
}
