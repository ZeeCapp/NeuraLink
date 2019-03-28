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
            Random rnd = new Random();
            List<double> traininset = new List<double>();
            List<double> trainioutset = new List<double>();

            for (int i = 0; i < 100000; i++)
            {
                int num1 = rnd.Next(0, 9);
                traininset.Add(num1);
                int num2 = rnd.Next(0, 9);
                traininset.Add(num2);

                trainioutset.Add(num1 + num2);
            }

            Task vypisovac;

            List<ActivationFunctions> activFunc = new List<ActivationFunctions>() {
                ActivationFunctions.ReLU,
                ActivationFunctions.ReLU,
                ActivationFunctions.ReLU,
                ActivationFunctions.ReLU
            };

            NeuralNetwork neuralNetwork = new NeuralNetwork(new int[] { 2, 10, 10, 1 }, 0.0001, activFunc);
            neuralNetwork.SaveNetworkAsXML(@"C:\Users\ZeeCaptain\Desktop\network.xml");

            Task networkTrainer = neuralNetwork.TrainAsync(traininset, trainioutset, 0.000001);
            Console.WriteLine("Training ...");

            Stopwatch watch = new Stopwatch();
            watch.Start();
            vypisovac = Task.Run(async () =>
            {
                while (networkTrainer.Status != TaskStatus.RanToCompletion)
                {
                    Console.WriteLine("Current error : " + neuralNetwork.AbsoluteError);
                    await Task.Delay(1);
                }
            });

            networkTrainer.Wait();
            watch.Stop();

            Console.WriteLine("Current error : " + neuralNetwork.AbsoluteError);
            Console.WriteLine("Done !   Training time - " + watch.Elapsed.ToString());

            List<double> output = neuralNetwork.Run(new List<double>(){1,4 });
            Console.WriteLine("{0}", output[0].ToString());

            Console.ReadKey();
        }
    }
}
