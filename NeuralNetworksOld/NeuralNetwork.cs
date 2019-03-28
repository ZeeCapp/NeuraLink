using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using System.Diagnostics;

namespace NeuralNetworks
{
    // TODO: saving and loading layers activation functions

    public enum ActivationFunctions { Sigmoid, ReLU };

    public class NeuralNetwork
    {
        public List<Layer> Layers { get; private set; }
        public double AbsoluteError { get; private set; }
        public delegate void ErrorCallback(string message);

        public object Lock = new object();

        private Task networkWeightsUpdaterTask;
        private Task<double> sumEffectOnOutputTask;
        private int selectedFunction;
        private int currentBackpropItteration;
        private double learningRate;
        private const double Epsilon = 0.0001;

        /// <summary>
        /// Generates a new neural network.
        /// </summary>
        /// <param name="layers">Number of neurons in each layer</param>
        /// <param name="activationFunction">Type of activation function used by the network</param>
        /// <param name="learningRate">Learning speed of the network</param>
        private NeuralNetwork(int[] layers, double learningRate, double bias, ActivationFunctions activationFunction)
        {
            this.Layers = new List<Layer>();
            this.selectedFunction = (int)activationFunction;
            this.learningRate = learningRate;
            InitiateNetwork(layers, bias, null);
        }

        private NeuralNetwork(List<Layer> layers)
        {
            this.Layers = layers;
        }

        public NeuralNetwork(int[] layers, double learningRate, double bias, List<ActivationFunctions> activationFunctions)
        {
            this.Layers = new List<Layer>();
            this.learningRate = learningRate;
            InitiateNetwork(layers, bias, activationFunctions);
        }


        public double GetAbsoluteError()
        {
            lock(Lock)
            {
                return this.AbsoluteError;
            }
        }

        /// <summary>
        /// Forward propagates input values through the network.
        /// </summary>
        /// <param name="InputValues">Input values for forward propagation</param>
        /// <returns></returns>
        public List<double> Run(List<double> InputValues)
        {
            List<double> outputValues = new List<double>();

            if(InputValues != null)
            {
                for(int l = 0; l < Layers.Count; l++)
                {
                    //sets the starting layer's neuron's output to the input values
                    if (l == 0)
                    {
                        for(int n = 0; n < Layers[l].Neurons.Count; n++)
                        {
                            Layers[l].Neurons[n].SetNeuronOutputSquashed(InputValues[n]);
                        }
                    }
                    //forward propagation
                    else
                    {
                        for (int n = 0; n < Layers[l].Neurons.Count; n++)
                        {
                            double output = 0;

                            for (int ln = 0; ln < Layers[l - 1].Neurons.Count; ln++)
                            {
                                output += Layers[l - 1].Neurons[ln].SquashedOutput * Layers[l].Neurons[n].Input[ln].Weight;
                            }

                            output += Layers[l].Neurons[n].Bias;
                            Layers[l].Neurons[n].SetNeuronOutput(output);
                            Layers[l].Neurons[n].SetNeuronOutputSquashed(Layers[l].activationFunction(output));
                            output = 0;
                        }
                    }
                }

                //saves the output to a list of doubles
                foreach(Neuron neuron in Layers[Layers.Count - 1].Neurons)
                {
                    outputValues.Add(neuron.SquashedOutput);
                }

                return outputValues;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Trains neural network until the target error is achieved.
        /// </summary>
        /// <param name="inputValues">Input data for training</param>
        /// <param name="targetValues">Wanted output</param>
        /// <param name="targetError">Target total error</param>
        /// <param name="callback">Method to be invoked after each back propagation cycle</param>
        public void Train(
            List<double> inputValues,
            List<double> targetValues,
            double targetError,
            ErrorCallback callback
            )
        {
            //BackpropCycleDoneEvent += UpdateNetworkWeights + callback;

            //checks if the neural network input/target values are plausible for current number of output neurons
            if (targetValues.Count % Layers[Layers.Count - 1].Neurons.Count != 0 || inputValues.Count % Layers[0].Neurons.Count !=0)
            {
                throw new InvalidTrainInputException();
            }
            else
            {
                currentBackpropItteration = 0;

                do
                {
                    //if the network weights are updating, wait for the update to finish
                    if (networkWeightsUpdaterTask != null)
                    {
                        networkWeightsUpdaterTask.Wait();
                    }

                    List<double> networkOutput = Run(inputValues.GetRange(currentBackpropItteration * Layers[0].Neurons.Count, Layers[0].Neurons.Count));

                    AbsoluteError = 0;
                    List<double> currentTargetValues = targetValues.GetRange(currentBackpropItteration * Layers[Layers.Count - 1].Neurons.Count, Layers[Layers.Count - 1].Neurons.Count);
                    int buffer = currentBackpropItteration * Layers[0].Neurons.Count + Layers[0].Neurons.Count;

                    if (buffer == inputValues.Count)
                    {
                        currentBackpropItteration = 0;
                    }
                    else
                    {
                        currentBackpropItteration++;
                    }
                    //calculates total network error
                    for (int n = 0; n < this.Layers[Layers.Count - 1].Neurons.Count; n++)
                    {                    
                        AbsoluteError += CostFunction(currentTargetValues[n], Layers[Layers.Count - 1].Neurons[n].SquashedOutput);
                    }

                    //back propagation main cycle
                    for (int l = this.Layers.Count - 1; l > 0; l--)
                    {
                        //update output layer
                        if (l == this.Layers.Count - 1)
                        {
                            for (int n = 0; n < this.Layers[l].Neurons.Count; n++)
                            {
                                //save the offect of this neuron on the total error for later use
                                Layers[l].Neurons[n].SetEffectOnOutput(OutputToErrorRatio(this.Layers[l].Neurons[n].SquashedOutput, currentTargetValues[n]));
                                for (int w = 0; w < this.Layers[l].Neurons[n].Input.Count; w++)
                                {
                                    double updateValue = this.Layers[l - 1].Neurons[w].SquashedOutput * Layers[l].activationFunctionDerivative(this.Layers[l].Neurons[n].Output) *
                                        this.Layers[l].Neurons[n].EffectOnOutput;

                                    Layers[l].Neurons[n].Input[w].UpdateDendriteWeight(updateValue * learningRate);
                                }
                            }
                        }
                        //update hiden layer
                        else
                        {
                            for (int n = 0; n < this.Layers[l].Neurons.Count; n++)
                            {
                                //save the offect of this neuron on the total error for later use

                                if(sumEffectOnOutputTask != null)
                                {
                                    sumEffectOnOutputTask.Wait();
                                }

                                sumEffectOnOutputTask = SumEffectOnErrorAsync(l, n);
                                Layers[l].Neurons[n].SetEffectOnOutput(sumEffectOnOutputTask.Result);

                                for (int w = 0; w < this.Layers[l].Neurons[n].Input.Count; w++)
                                {
                                    double updateValue = this.Layers[l - 1].Neurons[w].SquashedOutput * Layers[l].activationFunctionDerivative(this.Layers[l].Neurons[n].Output) * SumEffectOnError(l, n);

                                    this.Layers[l].Neurons[n].Input[w].UpdateDendriteWeight(updateValue * learningRate);
                                }
                            }
                        }
                    }
                    //start updating the network to new weight values
                    networkWeightsUpdaterTask = UpdateNetworkWeightsAsync();

                    callback?.Invoke("Training...  -  current error : " + AbsoluteError.ToString() + "\n");
                }
                while (AbsoluteError > targetError);

                if (sumEffectOnOutputTask != null)
                {
                    sumEffectOnOutputTask.Wait();
                }

                if (networkWeightsUpdaterTask != null)
                {
                    networkWeightsUpdaterTask.Wait();
                }

            }
        }


        /// <summary>
        /// Trains neural network asynchronously until the target error is achieved.
        /// </summary>
        /// <param name="inputValues">Input data for training</param>
        /// <param name="targetValues">Wanted output</param>
        /// <param name="targetError">Target total error</param>
        /// <param name="callback">Method to be invoked after each back propagation cycle</param>
        /// <returns>Instance of the Task class containing the Train method</returns>
        public Task TrainAsync(
            List<double> inputValues,
            List<double> targetValues,
            double targetError,
            ErrorCallback callback
            )
        {
            return Task.Factory.StartNew(() => Train(inputValues, targetValues, targetError,callback),TaskCreationOptions.LongRunning);
           // return Task.Run(() => Train(inputValues,targetValues,targetError));
        }

        /// <summary>
        /// Loads a neural network from a specified XML file
        /// </summary>
        /// <param name="path">XML file path.</param>
        /// <returns></returns>
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

                foreach(XmlNode layer in networkNode.ChildNodes)
                {
                    foreach(XmlNode neuron in layer.ChildNodes)
                    {
                        double bias = Double.Parse(neuron.Attributes["bias"].Value);
                        foreach(XmlNode input in neuron.ChildNodes)
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

        /// <summary>
        /// Loads a neural network from a specified XML file asynchronously
        /// </summary>
        /// <param name="path">XML file path</param>
        /// <returns></returns>
        public Task<NeuralNetwork> LoadNetworkFromXMLAsync(string path)
        {
            return Task.Run(() => LoadNetworkFromXML(path));
        }

        public static void SaveNetworkAsXML(string path, NeuralNetwork network)
        {
            XmlWriterSettings setting = new XmlWriterSettings();
            setting.Indent = true;
            Stream stream;

            stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write);

            using (XmlWriter writer = XmlWriter.Create(stream, setting))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("NeuralNetwork");
                writer.WriteAttributeString("learningRate", network.learningRate.ToString());
                foreach (Layer layer in network.Layers)
                {
                    writer.WriteStartElement("Layer");
                    writer.WriteAttributeString("activationFunction", layer.activationFunc.ToString());
                    foreach (Neuron neuron in layer.Neurons)
                    {
                        writer.WriteStartElement("Neuron");
                        writer.WriteAttributeString("bias", neuron.Bias.ToString());
                        foreach (Dendrite dendrite in neuron.Input)
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

        public static Task SaveNetworkAsXMLAsync(string path, NeuralNetwork network)
        {
            return new Task(() => SaveNetworkAsXML(path,network));
        }

        private double SumEffectOnError(int layer,int neuron)
        {
            double effect = 0;

            for(int n = 0; n < this.Layers[layer+1].Neurons.Count;n++)
            {
                effect += this.Layers[layer + 1].Neurons[n].Input[neuron].Weight * Layers[layer+1].activationFunctionDerivative(Layers[layer + 1].Neurons[n].Output) * Layers[layer + 1].Neurons[n].EffectOnOutput;
            }
            return effect;
        }

        private Task<double> SumEffectOnErrorAsync(int layer, int neuron)
        {
            return Task.Run(() => SumEffectOnError(layer, neuron));
        }

        private double OutputToErrorRatio(double actualOutput, double idealOutput)
        {
            return (actualOutput - idealOutput);
        }

        //Sigmoid activation function
        private double Sigmoid(double input)
        {
            double sigmoid;
            sigmoid =  1 / (1 + Math.Pow(Math.E, - input));
            return sigmoid;
        }

        private double SigmoidDerivate(double input)
        {
            double derivative;
            derivative = Sigmoid(input) * (1 - Sigmoid(input));
            return derivative;
        }

        //ReLU activation function
        private double ReLU(double x)
        {
            return Math.Max(0, x);
        }

        private double ReLUDerivative(double x)
        {
            double reluDerivative = 0;

            if (x > 0)
                reluDerivative = 1;

            return reluDerivative;
        }

        private double CostFunction(double target,double actual)
        {
            return (Math.Pow((actual - target),2));
        }

        //TODO finish this fucking bullshit
        private double CheckGradient(double currentWeight, double wantedOutput)
        {
            double difference = 0;


            return difference;
        }
      
        //updates all weights in the network 
        private void UpdateNetworkWeights()
        {
            for (int l = this.Layers.Count - 1; l > 0; l--)
            {
                for (int n = 0; n < this.Layers[l].Neurons.Count; n++)
                {
                    for (int w = 0; w < this.Layers[l].Neurons[n].Input.Count; w++)
                    {
                        Layers[l].Neurons[n].Input[w].ApplyChanges();
                    }
                }
            }
        }

        private Task UpdateNetworkWeightsAsync()
        {
            return Task.Run(() => UpdateNetworkWeights());
        }

        //Generates all layers and neurons and sets the activation function to be used
        private void InitiateNetwork(int[] layers,double bias, List<ActivationFunctions> activationFunctions)
        {
            if (layers != null)
            {
                for (int i = 0; i < layers.Length; i++)
                {
                    if (i == 0)
                    {
                        this.Layers.Add(new Layer(layers[i], 0, activationFunctions[i]));
                    }
                    else
                    {
                        if (activationFunctions == null)
                        {
                            this.Layers.Add(new Layer(layers[i], layers[i - 1], activationFunctions[i]));
                        }
                        else
                        {
                                this.Layers.Add(new Layer(layers[i], layers[i - 1], activationFunctions[i]));
                                if(activationFunctions[i] == ActivationFunctions.Sigmoid)
                                {
                                    Layers[i].SetAF(Sigmoid);
                                    Layers[i].SetAFDerivative(SigmoidDerivate);
                                }
                                else if (activationFunctions[i] == ActivationFunctions.ReLU)
                                {
                                    Layers[i].SetAF(ReLU);
                                    Layers[i].SetAFDerivative(ReLUDerivative);
                                }
                        }

                        foreach (Neuron neuron in Layers[i].Neurons)
                        {
                            neuron.SetBias(bias);
                        }
                    }
                }
            }
        }
    }

    [Serializable]
    public class InvalidTrainInputException : Exception
    {
        public InvalidTrainInputException()
        {

        }
    }

    [Serializable]
    public class InvalidAFListException : Exception
    {
        public InvalidAFListException()
        {

        }
    }
}
