using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using DataCollector;
using NeuralNetworks;

namespace DataCollectionTesting
{
    class Program
    {
        static void Main(string[] args)
        {
            //TODO  add updating dendrites using an avarage of all updates over a given number of periods, add network loading function

            Task vypisovac;

            List<ActivationFunctions> activFunc = new List<ActivationFunctions>() {
                ActivationFunctions.Sigmoid,
                ActivationFunctions.Sigmoid,
                ActivationFunctions.Sigmoid,
                ActivationFunctions.Sigmoid
            };

            NeuralNetwork neuralNetwork = new NeuralNetwork(new int[] { 2, 5, 10, 5, 2 }, 1, 0.1, activFunc);

            List<double> traininset = new List<double>();
            List<double> trainioutset = new List<double>();


            Task networkTrainer = neuralNetwork.TrainAsync(new List<double> { 0,0,1,1,0,1,1,0,}, new List <double> { 1,1,0,0,1,0,0,1}, 0.001);
            Console.WriteLine("Training ...");

            //Stopwatch watch = new Stopwatch();
            //watch.Start();
            vypisovac = Task.Run(async () =>
            {
                while (networkTrainer.Status != TaskStatus.RanToCompletion)
                {
                    Console.WriteLine("Current error : " + neuralNetwork.AbsoluteError);
                    await Task.Delay(1);
                }
            });

            networkTrainer.Wait();
            //watch.Stop();

            //Console.WriteLine("Current error : " + neuralNetwork.AbsoluteError);
            //Console.WriteLine("Done !   Training time - " + watch.Elapsed.ToString());

            NeuralNetwork.SaveNetworkAsXML(@"E:\Download\Network.XML", neuralNetwork);


            Console.WriteLine("Network saved !");

            Console.ReadKey();
        }
    }
}
