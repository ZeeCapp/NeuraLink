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
using System.Windows.Shapes;

namespace NeuraLink
{
    /// <summary>
    /// Interaction logic for SaveWarning.xaml
    /// </summary>
    public partial class SaveWarning : Window
    {
        public bool OverrideNetwork { get; private set; }

        public SaveWarning()
        {
            InitializeComponent();
            OverrideNetwork = false;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            OverrideNetwork = true;
            this.Close();
        }
    }
}
