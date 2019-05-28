using Microsoft.Win32;
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
    /// Interaction logic for NetworkPlain.xaml
    /// </summary>
    public partial class RunNetworkPage : Page
    {
        string selectedDataFile;
        MainWindow mainWindow;

        public RunNetworkPage(MainWindow mainWindow)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;
            this.Loaded += UpdateInputDisplay;
        }

        private void SelectDataButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog file = new OpenFileDialog();
            file.CheckFileExists = true;
            file.Multiselect = false;
            file.Filter = ".txt|*.txt|.csv|*.csv";

            if ((bool)file.ShowDialog())
            {
                selectedDataFile = file.FileName;
            }

            if (selectedDataFile != null || selectedDataFile != string.Empty)
            {
                SelectedDataTextBlock.Text = selectedDataFile;
            }
        }

        private void UpdateInputDisplay(object sender, RoutedEventArgs e)
        {
            if(mainWindow.neuralNetwork != null)
            {
                InputListBox.Items.Clear();
                for(int l = 1;l<=mainWindow.neuralNetwork.Layers[0].Neurons.Count;l++)
                {
                    InputListBox.Items.Add(new InputNeuron("Input " + l.ToString(), 0));
                }
            }
        }

        private class InputNeuron
        {
            public string Name { get; set; }
            public double Value { get; set; }

            public InputNeuron(string name, double value)
            {
                this.Name = name;
                this.Value = value;
            }
        }

        private class Output
        {
            public string NeuronNumber { get; set; }
            public double Value { get; set; }

            public Output(string neuronNumber, double value)
            {
                NeuronNumber = neuronNumber;
                Value = value;
            }
        }

        private void RunButton_Click(object sender, RoutedEventArgs e)
        {
            if(mainWindow.neuralNetwork != null)
            {
                List<double> inputs = new List<double>();

                for(int n=0;n<InputListBox.Items.Count;n++)
                {
                    inputs.Add((InputListBox.Items[n] as InputNeuron).Value);
                }

                List<double> outputsDouble = mainWindow.neuralNetwork.Run(inputs);

                List<Output> outputs = new List<Output>();
                int neuron = 1;
                foreach (double output in outputsDouble)
                {                   
                    outputs.Add(new Output(neuron.ToString(), output));
                    neuron++;
                }

                OutputsDataGrid.ItemsSource = outputs;
            }
        }
    }


}
