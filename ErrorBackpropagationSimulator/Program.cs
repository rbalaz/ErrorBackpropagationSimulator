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
            nineInputCase();
            Console.ReadLine();
        }

        static void nineInputCase()
        {
            Initialiser init = new Initialiser("4 13 1", 9);
            Network network = init.createMultiHiddenLayerNetwork();

            DataLoader loader = new DataLoader("data_2.csv");
            Data[] data = loader.loadDataFromFile();

            Learning learning = new Learning(1, data, network, 0.01);
            learning.executeLearningCycle(98.0);
        }

        static void continueLearning(string fileName)
        {
            Initialiser init = new Initialiser();

            Data[] trainingData;
            Data[] testingData;
            Network network = init.loadNetworkFromFile(fileName, out trainingData, out testingData);

            Learning learning = new Learning(trainingData, testingData, network);
            learning.executeLearningCycle(99.0);
        }

        static void retestNetwork(string fileName)
        {
            Initialiser init = new Initialiser();

            Data[] trainingData;
            Data[] testingData;
            Network network = init.loadNetworkFromFile(fileName, out trainingData, out testingData);

            Learning learning = new Learning(trainingData, testingData, network);
            learning.testTrainedNetwork();
        }

        static void demoExample()
        {
            Initialiser init = new Initialiser();
            Network network = init.createCV5Network();

            DataLoader loader = new DataLoader("test.txt");
            Data[] data = loader.loadDataFromFile();

            Learning learning = new Learning(0.8, data, network, 0.01);
            network.learningParameter = 0.8;
            learning.executeOneLearningCycle(data[0]);
        }
    }
}
