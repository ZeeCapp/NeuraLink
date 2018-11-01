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

namespace NeuraLink.Pages
{
    /// <summary>
    /// Interaction logic for TrainNetworkPage.xaml
    /// </summary>
    public partial class TrainNetworkPage : Page
    {
        private ListBox layerDisplay;
        private NeuralNetwork neuralNetwork;

        public TrainNetworkPage()
        {
            InitializeComponent();
            layerDisplay = (ListBox)this.FindName("LayersListBox");

            ListBoxItem item = new ListBoxItem();

            layerDisplay.Items.Add(FindResource("LayerDisplayItem"));
        }

        private void UpdateLayersButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow window = (MainWindow)MainWindow.GetWindow(this);
            neuralNetwork = window.neuralNetwork;
            layerDisplay.Items.Add(neuralNetwork);
        }

        private List<ListBoxItem> ConvertNetworkLayersToItems()
        {
            List<ListBoxItem> items = new List<ListBoxItem>();
            ListBoxItem item;
            foreach(Layer layer in neuralNetwork.Layers)
            {
                item = new ListBoxItem();
            }

            return items;
        }
    }
}
