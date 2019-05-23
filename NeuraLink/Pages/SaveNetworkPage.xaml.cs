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
using NeuralNetworks;
using MySql.Data.MySqlClient;

namespace NeuraLink.Pages
{
    /// <summary>
    /// Interaction logic for SaveNetworkPage.xaml
    /// </summary>
    public partial class SaveNetworkPage : Page
    {
        private MainWindow mainWindow;

        public SaveNetworkPage(MainWindow mainWindow)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;
            this.Loaded += LoadName;
        }

        private void LoadName(object sender, RoutedEventArgs e)
        {
            if(mainWindow.neuralNetwork!=null && mainWindow.neuralNetwork.Name != string.Empty)
            {
                NameTextBox.Text = mainWindow.neuralNetwork.Name;
            }
        }

        private void SaveAsXMLButton_Click(object sender, RoutedEventArgs e)
        {
            if(mainWindow.neuralNetwork != null)
            {
                SaveFileDialog browser = new SaveFileDialog();
                browser.AddExtension = true;
                browser.Filter = "XML (.xml)|*.xml";
                browser.ShowDialog();

                mainWindow.neuralNetwork.SaveNetworkAsXML(browser.FileName);
            }
        }

        private void LoadFromXMLButton_Click(object sender, RoutedEventArgs e)
        {
            if(mainWindow.neuralNetwork != null)
            {
                SaveWarning warning = new SaveWarning();
                warning.ShowDialog();

                if(warning.OverrideNetwork)
                {
                    OpenFileDialog file = new OpenFileDialog();
                    file.CheckFileExists = true;
                    file.Multiselect = false;
                    file.Filter = "XML (.xml)|*.xml";
                    file.ShowDialog();

                    mainWindow.neuralNetwork = NeuralNetwork.LoadNetworkFromXML(file.FileName);
                }
            }
            else
            {
                OpenFileDialog file = new OpenFileDialog();
                file.CheckFileExists = true;
                file.Multiselect = false;
                file.Filter = "XML (.xml)|*.xml";
                file.ShowDialog();

                mainWindow.neuralNetwork = NeuralNetwork.LoadNetworkFromXML(file.FileName);
            }
        }

        private void SaveToDatabaseButton_Click(object sender, RoutedEventArgs e)
        {
            if (mainWindow.neuralNetwork == null)
                return;

            mainWindow.neuralNetwork.Name = NameTextBox.Text;
            Connection connectionWindow = new Connection();
            connectionWindow.ShowDialog();

            string connString = "Server = " + connectionWindow.Address + ";";
            connString += "Database = " + connectionWindow.Database + ";";
            connString += "Uid = " + connectionWindow.Username + ";";
            connString += "Pwd = " + connectionWindow.Password + ";";

            MySqlConnection database = new MySqlConnection(connString);

            try
            {
                database.Open();

                string cmdText = "";
                MySqlCommand cmd = new MySqlCommand(null,database);
                MySqlDataReader reader;


                cmd.CommandText = "select * from NeuralNetwork where Name = \"" + mainWindow.neuralNetwork.Name + "\"";
                reader = cmd.ExecuteReader();
                if (reader.HasRows)
                    return;

                reader.Close();

                cmdText = "insert into NeuralNetwork(Name,LearningRate) values (\"" + mainWindow.neuralNetwork.Name + "\", " + mainWindow.neuralNetwork.LearningRate.ToString(System.Globalization.CultureInfo.InvariantCulture) + ");";
                cmd.CommandText = cmdText;
                cmd.ExecuteNonQuery();

                foreach (Layer layer in mainWindow.neuralNetwork.Layers)
                {
                    int currentLayerID=0;
                    cmdText = "insert into Layer (ActivationFunction,Name) values(\"" + layer.activationFunction.ToString() + "\",\"" + mainWindow.neuralNetwork.Name + "\");";
                    cmd.CommandText = cmdText;
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "select * from Layer where Name = \"" + mainWindow.neuralNetwork.Name + "\"";
                    reader = cmd.ExecuteReader();

                    while(reader.Read())
                    {
                        currentLayerID = reader.GetInt32("idLayer");
                    }
                    reader.Close();

                    foreach (Neuron neuron in layer.Neurons)
                    {
                        int currentNeuronID = 0;
                        cmdText = "insert into Neuron (Bias,idLayer) values(" + neuron.Bias.ToString(System.Globalization.CultureInfo.InvariantCulture) + "," + currentLayerID.ToString() + ");";
                        cmd.CommandText = cmdText;
                        cmd.ExecuteNonQuery();

                        cmd.CommandText = "select * from Neuron where idLayer = " + currentLayerID;
                        reader = cmd.ExecuteReader();

                        while (reader.Read())
                        {
                            currentNeuronID = reader.GetInt32("idNeuron");
                        }
                        reader.Close();

                        foreach (Dendrite dendrite in neuron.Dendrites)
                        {
                            cmdText = "insert into Dendrite (Weight,idNeuron) values(" + dendrite.Weight.ToString(System.Globalization.CultureInfo.InvariantCulture) + "," + currentNeuronID.ToString() + ");";
                            cmd.CommandText = cmdText;
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
                if (database.State == System.Data.ConnectionState.Open)
                    database.Close();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void LoadFromDatabaseButton_Click(object sender, RoutedEventArgs e)
        {
            NeuralNetwork network;
            List<int> keys = new List<int>();
            List<Layer> layers = new List<Layer>();
            List<Neuron> neurons = new List<Neuron>();
            Connection connectionWindow = new Connection();
            connectionWindow.ShowDialog();

            string connString = "Server = " + connectionWindow.Address + ";";
            connString += "Database = " + connectionWindow.Database + ";";
            connString += "Uid = " + connectionWindow.Username + ";";
            connString += "Pwd = " + connectionWindow.Password + ";";

            MySqlConnection connection = new MySqlConnection(connString);

            MySqlCommand cmd = new MySqlCommand(null, connection);
            MySqlDataReader reader;
            connection.Open();

            cmd.CommandText = "select * from Layer where Name = \"" + NameTextBox.Text + "\"";
            reader = cmd.ExecuteReader();
            while(reader.Read())
            {
                keys.Add(reader.GetInt32("idLayer"));
                layers.Add(new Layer(0, (ActivationFunctions)Enum.Parse(typeof(ActivationFunctions), reader.GetString("ActivationFunction"))));
            }
            reader.Close();

            foreach (int layer in keys)
            {
                cmd.CommandText = "select * from Neuron where idLayer = " + layer;
                reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    neurons.Add(new Neuron(reader.GetDouble("Bias")));
                }
            }

            if (connection.State == System.Data.ConnectionState.Open)
                connection.Close();
        }
    }
}
