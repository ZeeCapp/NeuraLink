using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace NeuralNetworks
{
    public enum ActivationFunctions { Sigmoid, ReLU };

    public class NeuralNetwork
    {
        public List<ActivationFunctions> activationFunctions;
        public double AbsoluteError { get; private set; }
        public List<Layer> Layers { get; set; }
        public double LearningRate { get; set; }
        public TimeSpan elapsed;
        public int LayerCount
        {
            get
            {
                return Layers.Count;
            }
        }       

        public NeuralNetwork(int[] layers, double learningRate, List<ActivationFunctions> activationFunctions)
        {
            if (layers.Length < 2) return;

            this.activationFunctions = activationFunctions;

            this.LearningRate = learningRate;
            this.Layers = new List<Layer>();

            for(int l = 0; l < layers.Length; l++)
            {
                Layer layer = new Layer(layers[l],activationFunctions[l]);
                this.Layers.Add(layer);

                for (int n = 0; n < layers[l]; n++)
                    layer.Neurons.Add(new Neuron());

                layer.Neurons.ForEach((nn) =>
                {
                    if (l == 0)
                        nn.Bias = 0;
                    else
                        for (int d = 0; d < layers[l - 1]; d++)
                            nn.Dendrites.Add(new Dendrite());
                });
            }
        }

        private NeuralNetwork(List<Layer> layers)
        {
            this.Layers = layers;
        }

        private double Sigmoid(double x)
        {
            return 1 / (1 + Math.Exp(-x));
        }

        private double SigmoidDerivative(double x)
        {
            return Sigmoid(x) * (1 - Sigmoid(x));
        }

        private double ReLU(double x)
        {
            return Math.Max(0, x);
        }

        private double ReLUDerivative(double x)
        {
            if (x > 0)
                return 0;
            return 1;
        }

        public List<double> Run(List<double> input)
        {
            if (input.Count != this.Layers[0].NeuronCount) return null;

            for (int l = 0; l < Layers.Count; l++)
            {
                Layer layer = Layers[l];

                for (int n = 0; n < layer.Neurons.Count; n++)
                {
                    Neuron neuron = layer.Neurons[n];

                    if (l == 0)
                        neuron.Value = input[n];
                    else
                    {
                        neuron.Value = 0;
                        for (int np = 0; np < this.Layers[l - 1].Neurons.Count; np++)
                            neuron.Value = neuron.Value + this.Layers[l - 1].Neurons[np].Value * neuron.Dendrites[np].Weight;

                        switch (Layers[l].activationFunction)
                        {
                            case ActivationFunctions.Sigmoid:
                                neuron.Value = Sigmoid(neuron.Value + neuron.Bias);
                                break;

                            case ActivationFunctions.ReLU:
                                neuron.Value = ReLU(neuron.Value + neuron.Bias);
                                break;
                        }                                         
                    }
                }
            }

            Layer last = this.Layers[this.Layers.Count - 1];
            int numOutput = last.Neurons.Count ;
            List<double> output = new List<double>();
            for (int i = 0; i < last.Neurons.Count; i++)
               output.Add(last.Neurons[i].Value);

            return output;
        }

        public bool Train(List<double> input, List<double> output, double target)
        {
            if ((input.Count % this.Layers[0].Neurons.Count) != 0 || ( input.Count % this.Layers[this.Layers.Count - 1].Neurons.Count) != 0) return false;

            int itteration = 0;

            Stopwatch clock = new Stopwatch();
            clock.Start();

            do
            {
                List<double> currentInput = input.GetRange(itteration * this.Layers[0].Neurons.Count, this.Layers[0].Neurons.Count);
                List<double> currentOutput = output.GetRange(itteration * this.Layers[this.Layers.Count - 1].Neurons.Count, this.Layers[this.Layers.Count - 1].Neurons.Count);
                Run(currentInput);

                for (int i = 0; i < this.Layers[this.Layers.Count - 1].Neurons.Count; i++)
                {
                    Neuron neuron = this.Layers[this.Layers.Count - 1].Neurons[i];

                    switch (Layers[this.Layers.Count - 1].activationFunction)
                    {
                        case ActivationFunctions.Sigmoid:
                            neuron.Delta = (currentOutput[i] - neuron.Value);
                            break;

                        case ActivationFunctions.ReLU:
                            neuron.Delta = (currentOutput[i] - neuron.Value);
                            break;
                    }

                    for (int j = this.Layers.Count - 2; j >= 1; j--)
                    {
                        for (int k = 0; k < this.Layers[j].Neurons.Count; k++)
                        {
                            Neuron n = this.Layers[j].Neurons[k];

                            switch (Layers[j].activationFunction)
                            {
                                case ActivationFunctions.Sigmoid:
                                    n.Delta = SigmoidDerivative(n.Value) *
                                      this.Layers[j + 1].Neurons[i].Dendrites[k].Weight *
                                      this.Layers[j + 1].Neurons[i].Delta;
                                    break;

                                case ActivationFunctions.ReLU:
                                    n.Delta = ReLUDerivative(n.Value) *
                                      this.Layers[j + 1].Neurons[i].Dendrites[k].Weight *
                                      this.Layers[j + 1].Neurons[i].Delta;
                                    break;
                            }
                        }
                    }
                }

                for (int i = this.Layers.Count - 1; i > 1; i--)
                {
                    for (int j = 0; j < this.Layers[i].Neurons.Count; j++)
                    {
                        Neuron n = this.Layers[i].Neurons[j];
                        n.Bias = n.Bias + (this.LearningRate * n.Delta);

                        for (int k = 0; k < n.Dendrites.Count; k++)
                            n.Dendrites[k].Weight = n.Dendrites[k].Weight + (this.LearningRate * this.Layers[i - 1].Neurons[k].Value * n.Delta);
                    }
                }

                AbsoluteError = 0;
                for (int n = 0; n < this.Layers[Layers.Count - 1].Neurons.Count; n++)
                {
                    AbsoluteError += Math.Abs(currentOutput[n] - Layers[Layers.Count - 1].Neurons[n].Value);
                }


                if((itteration+1) * this.Layers[0].Neurons.Count > input.Count - 1)
                {
                    itteration = 0;
                }
                else
                    itteration++;

                elapsed = clock.Elapsed;
            }
            while (AbsoluteError > target); //end of main cycle

            clock.Stop();
            elapsed = clock.Elapsed;

            return true;
        }

        public Task TrainAsync(List<double> input, List<double> output, double target)
        {
            return Task.Factory.StartNew(() => Train(input, output, target));
        }

        public void SaveNetworkAsXML(string path)
        {
            XmlWriterSettings setting = new XmlWriterSettings();
            setting.Indent = true;
            Stream stream;

            stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write);

            using (XmlWriter writer = XmlWriter.Create(stream, setting))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("NeuralNetwork");
                writer.WriteAttributeString("learningRate", LearningRate.ToString());
                foreach (Layer layer in Layers)
                {
                    writer.WriteStartElement("Layer");
                    writer.WriteAttributeString("activationFunction", layer.activationFunction.ToString());
                    foreach (Neuron neuron in layer.Neurons)
                    {
                        writer.WriteStartElement("Neuron");
                        writer.WriteAttributeString("bias", neuron.Bias.ToString());
                        foreach (Dendrite dendrite in neuron.Dendrites)
                        {
                            writer.WriteStartElement("InputDendrite");
                            writer.WriteAttributeString("weight", dendrite.Weight.ToString());
                            writer.WriteEndElement();
                        }
                        writer.WriteEndElement();
                    }
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
        }

        public static NeuralNetwork LoadNetworkFromXML(string path)
        {
            NeuralNetwork network;

            List<Neuron> neurons = new List<Neuron>();
            List<Layer> layers = new List<Layer>();
            List<Dendrite> inputs = new List<Dendrite>();

            using (StreamReader file = new StreamReader(path))
            {
                XmlReader reader = XmlReader.Create(file);

                XmlDocument document = new XmlDocument();
                document.Load(reader);

                XmlNode networkNode = document.SelectSingleNode("/NeuralNetwork");

                foreach (XmlNode layer in networkNode.ChildNodes)
                {
                    foreach (XmlNode neuron in layer.ChildNodes)
                    {
                        double bias = Double.Parse(neuron.Attributes["bias"].Value);
                        foreach (XmlNode input in neuron.ChildNodes)
                        {
                            inputs.Add(new Dendrite(Double.Parse(input.Attributes["weight"].Value)));
                        }
                        neurons.Add(new Neuron(inputs, bias));
                        inputs = new List<Dendrite>();
                    }
                    layers.Add(new Layer(neurons));
                    neurons = new List<Neuron>();
                }

                network = new NeuralNetwork(layers);
            }
            return network;
        }

    }
}
