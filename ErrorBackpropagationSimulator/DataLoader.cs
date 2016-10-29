using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrorBackpropagationSimulator
{
    class DataLoader
    {
        string loadFile;

        public DataLoader(string loadFile)
        {
            this.loadFile = loadFile;
        }

        public Data[] loadDataFromFile()
        {
            List<Data> data = new List<Data>();
            FileStream stream = new FileStream(loadFile, FileMode.Open, FileAccess.Read);
            StreamReader reader = new StreamReader(stream);

            string line;
            if(loadFile.Equals("data_2.csv"))
                line = reader.ReadLine(); // skipping first line
            while ((line = reader.ReadLine()) != null)
            {
                // String representing line from file now correctly splits.
                // Parsing string to double no correctly accepts format with dot 
                double[] parts = Array.ConvertAll(line.Split(','), s => double.Parse(s,System.Globalization.CultureInfo.InvariantCulture));
                Data vector = new Data(parts);
                data.Add(vector);
            }

            reader.Close();
            stream.Close();

            return data.ToArray();
        }
    }
}
