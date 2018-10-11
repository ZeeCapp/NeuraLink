using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NeuralNetworks
{
    public class Neuron
    {
        public List<Dendrite> Input { get; private set; }
        public double Output { get; private set; }
        public double SquashedOutput { get; private set; }
        public double Bias { get; private set; }
        public double EffectOnOutput { get; private set; }

        /// <summary>
        /// Initiates new neuron.
        /// </summary>
        /// <param name="numberOfDendrites">Number of connections coming from previous layer</param>
        public Neuron(int numberOfDendrites)
        {
            Input = new List<Dendrite>();
            InitiateNeuron(numberOfDendrites);
        }

        public Neuron(List<Dendrite> inputs, double bias)
        {
            this.Bias = bias;
            Input = inputs;
        }

        /// <summary>
        /// Sets a new output for this neuron.
        /// </summary>
        /// <param name="newOutput">Value of new output</param>
        public void SetNeuronOutputSquashed(double newSquashedOutput)
        {
            this.SquashedOutput = newSquashedOutput;
        }

        /// <summary>
        /// Sets a new output for this neuron.
        /// </summary>
        /// <param name="newOutput">Value of new output</param>
        public void SetNeuronOutput(double newOutput)
        {
            this.Output = newOutput;
        }

        public void SetEffectOnOutput(double effect)
        {
            this.EffectOnOutput = effect;
        }

        public void SetBias(double bias)
        {
            this.Bias = bias;
        }

        private void InitiateNeuron(int numberOfDendrites)
        {
            Random random = new Random();
            for (int i = 0; i < numberOfDendrites; i++)
            {
                Input.Add(new Dendrite());
            }

            this.Bias = 0.5;
        }

    }
}
