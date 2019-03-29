using NeuraLink.CustomControls;
using NeuraLink.Pages;
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
using NeuralNetworks;

namespace NeuraLink
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Graph graph;
        private RunNetworkPage currentRunPage;
        private TrainNetworkPage currentTrainPage;
        private SaveNetworkPage currentSavePage;
        private ListBox layerDisplay;

        public NeuralNetwork neuralNetwork;

        public MainWindow()
        {
            InitializeComponent();
            currentRunPage = new RunNetworkPage(this);
            currentTrainPage = new TrainNetworkPage(this);
            currentSavePage = new SaveNetworkPage();
        }

        private void TrainNetworkButton_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = null;
            MainFrame.Content = currentTrainPage;
            currentTrainPage.UpdateLayerDisplay(neuralNetwork);
            TrainNetworkButton.Background = new SolidColorBrush(Color.FromRgb(30, 30, 30));
            RunNetworkButton.Background = new SolidColorBrush(Color.FromRgb(35, 35, 35));

        }

        private void RunNetworkButton_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = null;

            MainFrame.Content = currentRunPage;
            graph = (Graph)MainFrame.FindName("DataGraph");
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
                if (graph != null)
                {
                    graph.CurrentData = new List<double> { 1, 4, 2, 8, 7, 2 };
                }
            }
        }

        private void SaveNetworkButton_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = null;

            MainFrame.Content = currentSavePage;
        }
    }
}
