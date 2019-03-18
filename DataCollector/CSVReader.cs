using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace DataCollector
{
    public class CSVReader
    {
        public static List<string> ReadCSVFile(string path)
        {
            List<string> output = new List<string>();

            StreamReader file = new StreamReader(path);

            while(!file.EndOfStream)
            {
                output.Add(file.ReadLine());
            }
            return output;
        }
    }
}
