﻿namespace NeuralNetworks
{
    public class Dendrite
    {
        public double Weight { get; set; }

        public Dendrite()
        {
            CryptoRandom n = new CryptoRandom();
            this.Weight = n.RandomValue;
        }

        public Dendrite(double weight)
        {
            this.Weight = weight;
        }
    }

}
