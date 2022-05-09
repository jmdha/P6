using ExperimentSuite.UserControls.MilestoneVisualiser.Controls;
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
using System.Windows.Shapes;

namespace ExperimentSuite.UserControls.MilestoneVisualiser
{
    /// <summary>
    /// Interaction logic for MilestoneVisualiser.xaml
    /// </summary>
    public partial class MilestoneVisualiser : Window
    {
        private IMilestoner _milestoner;

        public MilestoneVisualiser(IMilestoner milestoner, string titleName)
        {
            _milestoner = milestoner;
            InitializeComponent();
            Title = titleName;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (var attr in _milestoner.Milestones.Keys)
                MainPanel.Children.Add(new TableAttributeDataView(_milestoner, attr));
        }
    }
}
