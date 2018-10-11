using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace NeuralNetworks
{
    public class Dendrite
    {
        public double Weight { get; private set; }
        public const double weightMin = -0.2;
        public const double weightMax = 0.2;

        private double weightAfterBackPropagation;
        private Random rnd;


        //TODO make a method which updates all the dendrite weight after back propagation is done

        /// <summary>
        /// Initiates dendrite using a newly generated Random class. 
        /// </summary>
        public Dendrite()
        {
            rnd = new Random();
            InitiateDendrite(weightMin,weightMax);
        }

        public Dendrite(double weight)
        {
            this.Weight = weight;
        }

        /// <summary>
        /// Sets a new weight for this dendrite.
        /// </summary>
        /// <param name="newWeight">Value of new weight</param>
        public void UpdateDendriteWeight(double substractValue)
        { 
            this.weightAfterBackPropagation = Weight - substractValue;
        }

        public void ApplyChanges()
        {
            Weight = weightAfterBackPropagation;
            weightAfterBackPropagation = 0;
        }

        private void InitiateDendrite(double minimum, double maximum)
        {
            //creates a secure random double from <0,1>
            //using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            //{
            //    byte[] result = new byte[8];
            //    rng.GetBytes(result);
            //    this.Weight = ((double)BitConverter.ToDouble(result,0) / ulong.MaxValue) * (maximum-minimum) + minimum;
            //}

            Weight = rnd.NextDouble() * (maximum - minimum) + minimum;
        }
    }
}
