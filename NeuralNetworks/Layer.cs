using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetworks
{
    public class Layer
    { 
        public List<Neuron> Neurons { get; private set; }
        public Func<double,double> activationFunction;
        public Func<double,double> activationFunctionDerivative;

        /// <summary>
        /// Creates a new neural network layer with specified number of neurons
        /// and a specified number of connections for each neuron acording to the number of neurons in last layer.
        /// </summary>
        /// <param name="neurons">Number of neurons in this layer</param>
        /// <param name="lastLayerNeurons">Number of neurons in the last layer</param>
        public Layer(int neurons, int lastLayerNeurons)
        {
            this.Neurons = new List<Neuron>();
            this.InitiateNeurons(neurons,lastLayerNeurons);
        }

        public Layer(List<Neuron> neurons)
        {
            this.Neurons = neurons;
        }

        public void SetAF(Func<double, double> activationFunction)
        {
            this.activationFunction = activationFunction;
        }

        public void SetAFDerivative(Func<double, double> activationFunctionDerivative)
        {
            this.activationFunctionDerivative = activationFunctionDerivative;
        }

        private void InitiateNeurons(int numberOfNeurons,int lastLayerNeurons)
        {
            if (lastLayerNeurons == 0)
            {
                for (int i = 0; i < numberOfNeurons; i++)
                {
                    Neurons.Add(new Neuron(0));
                }
            }
            else
            {
                for (int i = 0; i < numberOfNeurons; i++)
                {
                    Neurons.Add(new Neuron(lastLayerNeurons));
                }
            }
        }
    }
}
