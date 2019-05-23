using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
            FileStream stream = new FileStream(@"C:\Users\ZeeCaptain\Desktop\Data.csv", FileMode.OpenOrCreate);
            stream.Close();
            string text = "";

            for (int i = 0; i < 100000; i++)
            {
                int num1 = rnd.Next(0, 9);
                traininset.Add(num1);
                int num2 = rnd.Next(0, 9);
                traininset.Add(num2);

                trainioutset.Add(num1 + num2);
                text += num1.ToString() + "," + num2.ToString() + "," + (num1 + num2).ToString() + Environment.NewLine;              
            }
            File.AppendAllText(@"C:\Users\ZeeCaptain\Desktop\TrainingData.csv", text);
        }
    }
}
