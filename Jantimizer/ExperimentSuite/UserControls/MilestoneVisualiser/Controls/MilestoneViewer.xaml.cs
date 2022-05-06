using Milestoner.Models.Milestones;
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

namespace ExperimentSuite.UserControls.MilestoneVisualiser.Controls
{
    /// <summary>
    /// Interaction logic for MilestoneViewer.xaml
    /// </summary>
    public partial class MilestoneViewer : UserControl
    {
        private IMilestone _milestone;
        public MilestoneViewer(IMilestone milestone)
        {
            _milestone = milestone;
            InitializeComponent();
            LowestValueLabel.Content = _milestone.LowestValue;
            HighestValueLabel.Content = _milestone.HighestValue;
            CountValueLabel.Content = _milestone.ElementsBeforeNextSegmentation;
        }

        private void LowerThanCombobox_DropDownOpened(object sender, EventArgs e)
        {
            if (LowerThanCombobox.Items.Count == 0)
                foreach(var value in _milestone.CountSmallerThan)
                    LowerThanCombobox.Items.Add($"{value.Key} : {value.Value}");
        }

        private void HigherThanCombobox_DropDownOpened(object sender, EventArgs e)
        {
            if (HigherThanCombobox.Items.Count == 0)
                foreach (var value in _milestone.CountLargerThan)
                    HigherThanCombobox.Items.Add($"{value.Key} : {value.Value}");
        }
    }
}
