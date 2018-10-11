﻿using NeuraLink.CustomControls;
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

namespace NeuraLink
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Graph graph;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void TrainNetworkButton_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = null;
            TrainNetworkButton.Background = new SolidColorBrush(Color.FromRgb(30, 30, 30));
            RunNetworkButton.Background = new SolidColorBrush(Color.FromRgb(35, 35, 35));

        }

        private void RunNetworkButton_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = null;

            MainFrame.Content = new RunNetworkPage();
            graph = (Graph)MainFrame.FindName("DataGraph");
            //MainFrame.Content = new RunNetworkPageLogic(MainFrame.ActualWidth, MainFrame.ActualHeight, new List<double> { 1,2,3,2,5,6,9,1,2});
            TrainNetworkButton.Background = new SolidColorBrush(Color.FromRgb(35, 35, 35));
            RunNetworkButton.Background = new SolidColorBrush(Color.FromRgb(30, 30, 30));
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            object content = MainFrame.Content;

            Page currentPage = (Page)content;
            if (currentPage != null)
            {
                Graph graph = (Graph)currentPage.FindName("DataGraph");
                graph.CurrentData = new List<double> { 1, 4, 2, 8, 7, 2 };
                graph.UpdateGraph(null,null);
            }

            //if (content is RunNetworkPageLogic)
            //{
            //    graph = (RunNetworkPageLogic)content;
            //}

            //if (graph != null)
            //{
            //    graph.UpdateGraph(MainFrame.ActualWidth);
            //}
        }
    }
}