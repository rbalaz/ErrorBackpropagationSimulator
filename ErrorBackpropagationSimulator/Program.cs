using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrorBackpropagationSimulator
{
    class Program
    {
        static void Main(string[] args)
        {
            demoExample();
            Console.ReadLine();
        }

        static void nineInputCase()
        {
            Initialiser init = new Initialiser("2 2 2 1", 9);
            Network network = init.createNeuralNetwork();

            DataLoader loader = new DataLoader("data_2.csv");
            Data[] data = loader.loadDataFromFile();

            Learning learning = new Learning(0.1, data, network);
            learning.executeLearningCycle();

            Console.WriteLine("Network testing success: " + learning.testTrainedNetwork());
        }

        static void demoExample()
        {
            Initialiser init = new Initialiser();
            Network network = init.createCV5Network();

            DataLoader loader = new DataLoader("test.txt");
            Data[] data = loader.loadDataFromFile();

            Learning learning = new Learning(0.8, data, network);
            network.learningParameter = 0.8;
            learning.executeOneLearningCycle(data[0]);
        }
    }
}
