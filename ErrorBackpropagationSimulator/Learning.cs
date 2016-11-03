using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Linq;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace ErrorBackpropagationSimulator
{
    class Learning
    {
        private double learningParameter;
        private Data[] trainingData;
        private Data[] testingData;
        private Network network;
        private double outputTolerance;

        public Learning(double learningParameter, Data[] data, Network network, double outputTolerance)
        {
            this.learningParameter = learningParameter;
            this.network = network;
            network.learningParameter = learningParameter;
            this.outputTolerance = outputTolerance;
            network.outputTolerance = outputTolerance;
            distributeData(shuffleData(data));
        }

        public Learning(Data[] trainingData, Data[] testingData, Network network)
        {
            this.trainingData = trainingData;
            this.testingData = testingData;
            this.network = network;
            learningParameter = network.learningParameter;
            outputTolerance = network.outputTolerance;
        }

        public void executeLearningCycle(double successRate)
        {
            int correctClass1;
            int correctClass2;
            double globalError;
            Stopwatch watch = new Stopwatch();
            int iterationCounter = 1;
            do
            {
                watch.Start();
                correctClass1 = 0;
                correctClass2 = 0;
                globalError = 0;

                for (int i = 0; i < trainingData.Length; i++)
                {
                    network.propagate(trainingData[i]);
                    if (network.currentIterationSuccess)
                    {
                        if (trainingData[i].ev == 0)
                            correctClass1++;
                        else
                            correctClass2++;
                    }
                    globalError += network.currentIterationError * network.currentIterationError;
                }
                changeLearningParameter(evaluateNetworkSuccess(correctClass1, correctClass1, trainingData));
                network.learningParameter = learningParameter;
                watch.Stop();
                Console.WriteLine(iterationCounter++ + ": Learning Success: " + evaluateNetworkSuccess(correctClass1, correctClass2, trainingData) + " Time elapsed: " + watch.Elapsed);
                watch.Reset();
            } while (evaluateNetworkSuccess(correctClass1, correctClass2, trainingData) < successRate);
            printContingencyTable(correctClass1, correctClass2, "training");
            testTrainedNetwork();
            saveNeuralNetwork();
        }

        public void executeOneLearningCycle(Data data)
        {
            network.propagate(data);
        }

        public void testTrainedNetwork()
        {
            int correctClass1 = 0;
            int correctClass2 = 0;

            for (int i = 0; i < testingData.Length; i++)
            {
                if (network.compute(testingData[i]) == testingData[i].ev)
                {
                    if (testingData[i].ev == 0)
                        correctClass1++;
                    else
                        correctClass2++;
                }
                Console.WriteLine(i + " Expected value:" + testingData[i].ev + " Calculated value: " + network.layers[network.layers.Count - 1][0].output + " " + network.currentIterationSuccess);
            }
            printContingencyTable(correctClass1, correctClass2, "testing");
        }

        private static Data[] shuffleData(Data[] data)
        {
            Data[] shuffledData = data;
            for (int i = 0; i < data.Length; i++)
            { 
                Random generator = new Random();
                int random;
                bool uniqueNumberGenerated;
                do
                {
                    random = generator.Next(data.Length);
                    uniqueNumberGenerated = false;
                    if(i != random)
                    {
                        Data temp;
                        temp = shuffledData[random];
                        shuffledData[random] = shuffledData[i];
                        shuffledData[i] = temp;
                        uniqueNumberGenerated = true;
                    }
                } while (uniqueNumberGenerated);
            }
            return shuffledData;
        }

        private void distributeData(Data[] data)
        {
            trainingData = new Data[(int)Math.Ceiling(data.Length * 0.8)];
            // fixed filling training data raising InvalidCastException
            trainingData = data.Take(trainingData.Length).ToArray();
            testingData = new Data[data.Length - trainingData.Length];
            // testing data are now correctly filled without rasing ArgumentException
            Array.Copy(data, trainingData.Length, testingData, 0, data.Length - trainingData.Length);
        }

        private void changeLearningParameter(double successRate)
        {
            // 95 and more...0.01
            // 50 and less... 10

            if (successRate < 50)
                learningParameter = 10;
            else if (successRate > 95)
                learningParameter = 0.01;
            else
                learningParameter = -0.222 * successRate + 21.1;
        }

        private double evaluateNetworkSuccess( double correctClass1, double correctClass2, Data[] data)
        {
            double class1Success = correctClass1 / data.Count(item => item.ev == 0);
            double class2Success = correctClass2 / data.Count(item => item.ev == 1);

            return 50 * (class1Success + class2Success);
        }

        private void printContingencyTable(int correctClassA, int correctClassB, string type)
        {
            if (type.Equals("training"))
            {
                Console.WriteLine(">>>Contingency table for NN {0}<<<", type);
                Console.WriteLine("--------------------");
                Console.WriteLine("| ev/x |  a  |  b  |");
                Console.WriteLine("--------------------");
                Console.WriteLine("|   a  | {0} | {1} |", correctClassA, trainingData.Count(item => item.ev == 0) - correctClassA);
                Console.WriteLine("--------------------");
                Console.WriteLine("|   b  | {0} | {1} |", correctClassB, trainingData.Count(item => item.ev == 1) - correctClassB);
                Console.WriteLine("--------------------");
            }
            if (type.Equals("testing"))
            {
                Console.WriteLine(">>>Contingency table for NN {0}<<<", type);
                Console.WriteLine("--------------------");
                Console.WriteLine("| ev/x |  a  |  b  |");
                Console.WriteLine("--------------------");
                Console.WriteLine("|   a  | {0} | {1} |", correctClassA, testingData.Count(item => item.ev == 0) - correctClassA);
                Console.WriteLine("--------------------");
                Console.WriteLine("|   b  | {0} | {1} |", correctClassB, testingData.Count(item => item.ev == 1) - correctClassB);
                Console.WriteLine("--------------------");
            }
        }

        private void saveNeuralNetwork()
        {
            string name = (new Random()).Next(2345678).ToString();
            name = string.Concat(name, ".txt");
            FileStream stream = new FileStream(name, FileMode.OpenOrCreate, FileAccess.Write);
            StreamWriter writer = new StreamWriter(stream);

            string init = "";
            foreach (Neuron[] n in network.layers)
            {
                init = string.Concat(init, n.Length + " ");
            }
            init = init.Trim(' ');
            writer.WriteLine(init);
            writer.WriteLine(network.layers[0][0].f.type + " " + network.layers[0][0].f.alfa);
            writer.WriteLine(learningParameter);
            writer.WriteLine(network.outputTolerance);

            int counter = 1;
            foreach (Neuron[] n in network.layers)
            {
                writer.WriteLine("Layer " + counter++ + ":");
                for (int i = 0; i < n.Length; i++)
                {
                    writer.WriteLine("Neuron " + (i + 1) + ":");
                    string weights = "";
                    foreach (Synapse s in n[i].synapses)
                    {
                        weights = string.Concat(weights, s.getWeight() + " ");
                    }
                    weights = weights.Trim(' ');
                    writer.WriteLine(weights);
                }
            }

            writer.WriteLine("Training data");
            for (int i = 0; i < trainingData.Length; i++)
            {
                writer.WriteLine(trainingData[i]);
            }
            writer.WriteLine("Testing data");
            for (int i = 0; i < testingData.Length; i++)
            {
                writer.WriteLine(testingData[i]);
            }

            writer.Close();
            stream.Close();
        }
    }
}
