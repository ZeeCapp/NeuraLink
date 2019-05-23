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
using Microsoft.Win32;
using NeuralNetworks;
using DataCollector;
using System.Threading;
using System.Text.RegularExpressions;

namespace NeuraLink.Pages
{
    /// <summary>
    /// Interaction logic for TrainNetworkPage.xaml
    /// </summary>
    public partial class TrainNetworkPage : Page
    {
        private ListBox layerDisplay;
        private NeuralNetwork neuralNetwork;
        MainWindow window;
        TextBox consoleTextBox;

        public string selectedTrainingFile { get; private set; }

        public TrainNetworkPage(MainWindow parrent)
        {
            InitializeComponent();
            layerDisplay = (ListBox)this.FindName("LayersListBox");
            window = parrent;
            neuralNetwork = parrent.neuralNetwork;
            layerDisplay.Items.Add(new ListBoxItem());
            this.consoleTextBox = new TextBox();
            consoleTextBox.Style = (Style)Application.Current.Resources["ConsoleStyle"];
            ((Grid)this.FindName("TrainNetworkPageGrid")).Children.Add(consoleTextBox);
        }

        public void UpdateLayerDisplay(NeuralNetwork neuralNetwork)
        {
            if (neuralNetwork != null && layerDisplay.Items.Count-1 != neuralNetwork.Layers.Count)
            {
                for (int l = 1; l <= neuralNetwork.Layers.Count; l++)
                {
                    layerDisplay.Items.Add(new NetworkLayerDescriptor("Layer " + l.ToString(), neuralNetwork.Layers[l - 1].Neurons.Count, neuralNetwork.Layers[l - 1].activationFunction));
                }
            }
        }

        private void UpdateLayersButton_Click(object sender, RoutedEventArgs e)
        {
            int layersCount = 0;
            neuralNetwork = window.neuralNetwork;

            if (!int.TryParse(LayersNumber.Text, out layersCount))
            {
                consoleTextBox.AppendText("Invalid layers count ! \n");
                return;
            }

            if (layerDisplay.Items.Count - 1 > int.Parse(LayersNumber.Text))
            {
                for (int position = layerDisplay.Items.Count - 1; position > int.Parse(LayersNumber.Text); position--)
                {
                    layerDisplay.Items.RemoveAt(position);
                }
            }
            else if (layerDisplay.Items.Count - 1 < int.Parse(LayersNumber.Text))
            {
                for (int position = layerDisplay.Items.Count - 1; position < int.Parse(LayersNumber.Text); position++)
                {
                    layerDisplay.Items.Add(new NetworkLayerDescriptor("Layer " + (position + 1).ToString(), 0, ActivationFunctions.Sigmoid));
                }
            }
        }

        public class NetworkLayerDescriptor
        {
            public string layerName { get; set; }
            public int neurons { get; set; }
            public int selectedIndex { get; set; }

            public NetworkLayerDescriptor(string name, int neuronsNumber, ActivationFunctions activationFunction)
            {
                layerName = name;
                neurons = neuronsNumber;
                selectedIndex = (int)activationFunction;
            }
        }

        private void TrainingDataButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog file = new OpenFileDialog();
            file.CheckFileExists = true;
            file.Multiselect = false;
            file.Filter = ".txt|*.txt|.csv|*.csv";

            if ((bool)file.ShowDialog())
            {
                selectedTrainingFile = file.FileName;
            }

            if (selectedTrainingFile != null || selectedTrainingFile != string.Empty)
            {
                SelectedDataTextBlock.Text = selectedTrainingFile;
            }
        }

        private void CreateNetworkButton_Click(object sender, RoutedEventArgs e)
        {
            //check if learning rate is set
            double learningRate;
            if (!Double.TryParse(LearningRateTextBox.Text, out learningRate))
            {
                consoleTextBox.AppendText("Learning rate must be set and must be in a correct format (x,xxx) ! \n");
                return;
            }

            for (int lay = 1; lay < layerDisplay.Items.Count; lay++)
            {
                if ((layerDisplay.Items[lay] as NetworkLayerDescriptor).neurons == 0)
                {
                    consoleTextBox.AppendText("Network´s layer cant have 0 neurons ! \n");
                    return;
                }
            }

                neuralNetwork = window.neuralNetwork;
            SaveWarning saveWarning;

            //if there allready is a network, ask user to save it
            if (neuralNetwork != null)
            {                
                saveWarning = new SaveWarning();
                saveWarning.ShowDialog();

                if (saveWarning.OverrideNetwork)
                {
                    List<int> layers = new List<int>();
                    List<ActivationFunctions> activationFunctions = new List<ActivationFunctions>();

                    for (int lay = 1; lay < layerDisplay.Items.Count; lay++)
                    {
                        layers.Add((layerDisplay.Items[lay] as NetworkLayerDescriptor).neurons);
                        activationFunctions.Add((ActivationFunctions)Enum.Parse(typeof(ActivationFunctions), (layerDisplay.Items[lay] as NetworkLayerDescriptor).selectedIndex.ToString()));
                    }

                    window.neuralNetwork = new NeuralNetwork(layers.ToArray(), learningRate, activationFunctions);
                    this.neuralNetwork = window.neuralNetwork;

                    consoleTextBox.AppendText("Network succesfully created ! \n");
                }
                else
                {
                    consoleTextBox.AppendText("Creation canceled \n");
                }
            }
            //if there is no network, create a new one
            else
            {
                List<int> layers = new List<int>();
                List<ActivationFunctions> activationFunctions = new List<ActivationFunctions>();

                for (int lay = 1; lay < layerDisplay.Items.Count; lay++)
                {
                    layers.Add((layerDisplay.Items[lay] as NetworkLayerDescriptor).neurons);
                    activationFunctions.Add((ActivationFunctions)Enum.Parse(typeof(ActivationFunctions), (layerDisplay.Items[lay] as NetworkLayerDescriptor).selectedIndex.ToString()));
                }

                window.neuralNetwork = new NeuralNetwork(layers.ToArray(), learningRate, activationFunctions);
                this.neuralNetwork = window.neuralNetwork;

                consoleTextBox.AppendText("Network succesfully created ! \n");
            }
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            this.neuralNetwork = window.neuralNetwork;

            if (selectedTrainingFile == null || selectedTrainingFile == string.Empty)
            {
                consoleTextBox.AppendText("No training data selected !\n");
                return;
            }

            if (window.neuralNetwork == null)
            {
                consoleTextBox.AppendText("No network loaded !\n");
                return;
            }

            double buffer;
            TimeSpan timespanBuffer;

            if(errorTargetTextBox.Text == string.Empty && !TimeSpan.TryParse(LearningTimeTextBox.Text,out timespanBuffer)
                || errorTargetTextBox.Text == string.Empty && TimeSpan.Parse(LearningTimeTextBox.Text) == TimeSpan.Zero ||
                !Double.TryParse(errorTargetTextBox.Text,out buffer) && TimeSpan.Parse(LearningTimeTextBox.Text) == TimeSpan.Zero ||
                !Double.TryParse(errorTargetTextBox.Text, out buffer) && !TimeSpan.TryParse(LearningTimeTextBox.Text, out timespanBuffer))
            {
                consoleTextBox.AppendText("Invalid error target !\n");
                return;
            }


            List<double> inputValues = new List<double>();
            List<double> outputValues = new List<double>();

            try
            {
                List<string> readData = CSVReader.ReadCSVFile(selectedTrainingFile);

                for (int line = 0; line < readData.Count; line++)
                {
                    string[] splitData = readData[line].Split(',');

                    for (int data = 0; data < splitData.Length; data++)
                    {
                        if (data < neuralNetwork.Layers[0].Neurons.Count)
                            inputValues.Add(double.Parse(splitData[data]));
                        else
                            outputValues.Add(double.Parse(splitData[data]));
                    }
                }
            }
            catch(Exception exc)
            {
                consoleTextBox.AppendText("Invalid training data !");
                return;
            }

            Task networkTrainer;
            TimeSpan span = TimeSpan.Parse(LearningTimeTextBox.Text);

            if(errorTargetTextBox.Text == string.Empty && span.TotalMilliseconds >= 0)
            {
                networkTrainer = neuralNetwork.TrainAsync(inputValues, outputValues, span);
            }
            else if (span.TotalMilliseconds >= 0)
            {
                networkTrainer = neuralNetwork.TrainAsync(inputValues, outputValues, double.Parse(errorTargetTextBox.Text), span);
            }
            else
            {
                networkTrainer = neuralNetwork.TrainAsync(inputValues, outputValues, double.Parse(errorTargetTextBox.Text));
            }

            Task errorDisplayer = Task.Factory.StartNew(async () =>
            {
                while (networkTrainer.Status == TaskStatus.WaitingToRun
                || networkTrainer.Status == TaskStatus.WaitingForActivation
                || networkTrainer.Status == TaskStatus.WaitingForChildrenToComplete
                || networkTrainer.Status == TaskStatus.Running
                || networkTrainer.Status == TaskStatus.Created)
                {
                    if (neuralNetwork.AbsoluteError != 0)
                    {
                        this.Dispatcher.Invoke(() =>
                        {
                            consoleTextBox.AppendText("Training...   current error: " + neuralNetwork.AbsoluteError.ToString() + "      Elapsed: " + neuralNetwork.elapsed.TotalMilliseconds + " milliseconds" + Environment.NewLine);
                            consoleTextBox.ScrollToEnd();
                        });
                    }                    
                   await Task.Delay(300);
                }
                this.Dispatcher.Invoke(() =>
                {
                    consoleTextBox.AppendText("Training finished.\n");
                });
            });

        }
    }
}
