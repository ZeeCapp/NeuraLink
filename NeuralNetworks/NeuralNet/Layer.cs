using System.Collections.Generic;

namespace NeuralNetworks
{
    public class Layer
    {
        public List<Neuron> Neurons { get; set; }
        public ActivationFunctions activationFunction { get; private set; }
        public int NeuronCount
        {
            get
            {
                return Neurons.Count;
            }
        }

        public Layer(int numNeurons, ActivationFunctions activationFunction)
        {
            this.activationFunction = activationFunction;
            Neurons = new List<Neuron>(numNeurons);
        }

        public Layer(List<Neuron> neurons, ActivationFunctions activationFunction)
        {
            this.Neurons = neurons;
            this.activationFunction = activationFunction;
        }
    }
}
