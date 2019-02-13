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

            layerDisplay.Items.Add(new ListBoxItem());
        }

        public void UpdateLayerDisplay(NeuralNetwork neuralNetwork)
        {
            for(int l = 1; l <= neuralNetwork.Layers.Count; l++)
            {
                layerDisplay.Items.Add(new NetworkLayerDescriptor("Layer " + l.ToString(),neuralNetwork.Layers[l-1].Neurons.Capacity));
            }
        }

        private void UpdateLayersButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow window = (MainWindow)MainWindow.GetWindow(this);
            neuralNetwork = window.neuralNetwork; 
            
            if(layerDisplay.Items.Count - 1 > int.Parse(LayersNumber.Text))
            {
                for (int position = layerDisplay.Items.Count - 1; position > int.Parse(LayersNumber.Text); position--)
                {
                    layerDisplay.Items.RemoveAt(position);
                }
            }
            else if(layerDisplay.Items.Count-1 < int.Parse(LayersNumber.Text))
            {
                for (int position = layerDisplay.Items.Count-1; position < int.Parse(LayersNumber.Text); position++)
                {
                    layerDisplay.Items.Add(new NetworkLayerDescriptor("Layer " + (position + 1).ToString(),0));
                }
            }
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

        public class NetworkLayerDescriptor
        {
            public string layerName { get; set; }
            public int neurons { get; set; }

            public NetworkLayerDescriptor(string name,int neuronsNumber)
            {
                layerName = name;
                neurons = neuronsNumber;
            }
        }
    }
}
