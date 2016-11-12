using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ErrorBackpropagationMVC
{
    class Learning
    {
        public class Class
        {
            public int representingValue { get; set; }
            public int correctlyClassified { get; set; }

            public Class(int representingValue)
            {
                this.representingValue = representingValue;
            }
        }

        private double learningParameter;
        private Data[] trainingData;
        private Data[] testingData;
        private Network network;
        private double outputTolerance;
        public double[][] learningTable { get; private set; }
        public double[][] testingTable { get; private set; }
        public List<double> learningRate { get; private set; }
        public List<double> globalErrorRate { get; private set; }
        public string folderPath { get; set; }

        public Learning(double learningParameter, Data[] data, Network network, double outputTolerance)
        {
            this.learningParameter = learningParameter;
            this.network = network;
            network.learningParameter = learningParameter;
            this.outputTolerance = outputTolerance;
            network.outputTolerance = outputTolerance;
            learningRate = new List<double>();
            globalErrorRate = new List<double>();
            distributeData(shuffleData(data));
        }

        public Learning(Data[] trainingData, Data[] testingData, Network network)
        {
            this.trainingData = trainingData;
            this.testingData = testingData;
            this.network = network;
            learningParameter = network.learningParameter;
            outputTolerance = network.outputTolerance;
            learningRate = new List<double>();
        }

        public void executeLearningCycle(double successRate)
        {
            List<Class> classes = new List<Class>();
            processData(classes, trainingData.Concat(testingData).ToArray());
            double globalError;
            do
            {
                foreach (Class c in classes)
                    c.correctlyClassified = 0;
                globalError = 0;

                for (int i = 0; i < trainingData.Length; i++)
                {
                    network.propagate(trainingData[i]);
                    if (network.currentIterationSuccess)
                    {
                        classes.Find(f => f.representingValue == trainingData[i].ev).correctlyClassified++;
                    }
                    globalError += network.currentIterationError * network.currentIterationError;
                }
                changeLearningParameter(evaluateNetworkSuccess(classes, trainingData));
                network.learningParameter = learningParameter;
                globalErrorRate.Add(globalError);
                learningRate.Add(evaluateNetworkSuccess(classes, trainingData));
            } while (evaluateNetworkSuccess(classes, trainingData) < successRate);
            setContingencyTable(classes, "training");
            testTrainedNetwork();
            saveNeuralNetwork();
        }

        private void processData(List<Class> classes, Data[] data)
        {
            for (int i = 0; i < data.Length; i++)
            {
                if (classes.FindIndex(f => f.representingValue == data[i].ev) == -1)
                    classes.Add(new Class(data[i].ev));
            }
        }

        public void testTrainedNetwork()
        {
            List<Class> classes = new List<Class>();
            processData(classes, trainingData.Concat(testingData).ToArray());
            foreach (Class c in classes)
                c.correctlyClassified = 0;
            for (int i = 0; i < testingData.Length; i++)
            {
                if (network.compute(testingData[i]) == testingData[i].ev)
                {
                    classes.Find(f => f.representingValue == testingData[i].ev).correctlyClassified++;
                }
            }
            setContingencyTable(classes, "testing");
        }

        public void retestTrainedNetwork()
        {
            List<Class> classes = new List<Class>();
            processData(classes, trainingData.Concat(testingData).ToArray());
            foreach (Class c in classes)
                c.correctlyClassified = 0;

            for (int i = 0; i < trainingData.Length; i++)
            {
                if (network.compute(trainingData[i]) == trainingData[i].ev)
                {
                    classes.Find(f => f.representingValue == trainingData[i].ev).correctlyClassified++;
                }
            }
            setContingencyTable(classes, "training");

            foreach (Class c in classes)
                c.correctlyClassified = 0;
            for (int i = 0; i < testingData.Length; i++)
            {
                if (network.compute(testingData[i]) == testingData[i].ev)
                {
                    classes.Find(f => f.representingValue == testingData[i].ev).correctlyClassified++;
                }
            }
            setContingencyTable(classes, "testing");
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
            trainingData = data.Take(trainingData.Length).ToArray();
            testingData = new Data[data.Length - trainingData.Length];
            Array.Copy(data, trainingData.Length, testingData, 0, data.Length - trainingData.Length);
        }

        private void changeLearningParameter(double successRate)
        {
            if (successRate >= 95)
                learningParameter = 0.05;
            else if (successRate >= 50)
                learningParameter = (-17.0 / 900.0) * successRate + (83.0 / 45.0);
            else
                learningParameter = 0.9;
        }

        private double evaluateNetworkSuccess(List<Class> classes, Data[] data)
        {
            double success = 0;
            foreach (Class c in classes)
            {
                success += (double)c.correctlyClassified / (double)data.Count(item => item.ev == c.representingValue);
            }
            return success / classes.Count * 100;
        }

        private void setContingencyTable(List<Class> classes, string type)
        {
            if (type.Equals("training"))
            {
                learningTable = new double[classes.Count][];
                for (int i = 0; i < classes.Count; i++)
                    learningTable[i] = new double[2];

                for (int i = 0; i < classes.Count; i++)
                { 
                    learningTable[i][0] = trainingData.Count(item => item.ev == classes[i].representingValue);
                    learningTable[i][1] = classes[i].correctlyClassified;
                }
            }

            if (type.Equals("testing"))
            {
                testingTable = new double[classes.Count][];
                for(int i = 0; i < classes.Count; i++)
                    testingTable[i] = new double[2];

                for (int i = 0; i < classes.Count; i++)
                {
                    testingTable[i][0] = testingData.Count(item => item.ev == classes[i].representingValue);
                    testingTable[i][1] = classes[i].correctlyClassified;
                }
            }
        }

        private void saveNeuralNetwork()
        {
            string name = (new Random()).Next(2345678).ToString();
            name = string.Concat(name, ".txt");
            string filePath = Path.Combine(folderPath, name);
            FileStream stream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write);
            StreamWriter writer = new StreamWriter(stream);

            string init = "";
            foreach (Neuron[] n in network.layers)
            {
                init = string.Concat(init, n.Length + " ");
            }
            init = init.Trim(' ');
            writer.WriteLine(init);
            writer.WriteLine(network.layers[0][0].f.alfa);
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
