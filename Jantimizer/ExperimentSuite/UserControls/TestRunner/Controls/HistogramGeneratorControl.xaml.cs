﻿using System;
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
    /// Interaction logic for HistogramGeneratorControl.xaml
    /// </summary>
    public partial class HistogramGeneratorControl : UserControl
    {
        public HistogramGeneratorControl()
        {
            InitializeComponent();
        }

        private void HistogramProgressBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            HistogramProgressLabel.Content = $"Histogram Progress ({HistogramProgressBar.Value}/{HistogramProgressBar.Maximum})";
        }
    }
}
