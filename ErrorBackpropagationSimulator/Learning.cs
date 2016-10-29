using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
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

        public Learning(double learningParameter, Data[] data, Network network)
        {
            this.learningParameter = learningParameter;
            this.network = network;
            distributeData(shuffleData(data));
        }

        public void executeLearningCycle()
        {
            int correctClass1;
            int correctClass2;
            do
            {
                correctClass1 = 0;
                correctClass2 = 0;

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
                }
                Console.WriteLine("Learning Success: " + evaluateNetworkSuccess(correctClass1, correctClass2, trainingData));
            } while (evaluateNetworkSuccess(correctClass1, correctClass2, trainingData) < 95.0);
        }

        public void executeOneLearningCycle(Data data)
        {
            network.propagate(data);
        }

        public double testTrainedNetwork()
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
            }
            return evaluateNetworkSuccess(correctClass1, correctClass2, testingData);
        }

        private static Data[] shuffleData(Data[] data)
        {
            Data[] shuffledData = data;
            List<int> usedNumbers = new List<int>();
            for (int i = 0; i < data.Length; i++)
            {
                if (usedNumbers.IndexOf(i) != -1)
                    continue;
                if (data.Length - usedNumbers.Count == 1)
                    break;

                Random generator = new Random();
                int random;
                bool uniqueNumberGenerated;
                do
                {
                    random = generator.Next(data.Length);
                    uniqueNumberGenerated = false;
                    if (usedNumbers.IndexOf(random) == -1)
                    {
                        usedNumbers.Add(random);
                        usedNumbers.Add(i);
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

        // This needs testing, I have no clue if these functions actually work as intended
        private void distributeData(Data[] data)
        {
            trainingData = new Data[(int)Math.Ceiling(data.Length * 0.8)];
            // fixed filling training data raising InvalidCastException
            trainingData = data.Take(trainingData.Length).ToArray();
            testingData = new Data[data.Length - trainingData.Length];
            // testing data are now correctly filled without rasing ArgumentException
            Array.Copy(data, trainingData.Length, testingData, 0, data.Length - trainingData.Length);
        }

        private double evaluateNetworkSuccess(int correctClass1, int correctClass2, Data[] data)
        {
            int class1Success = correctClass1 / data.Count(item => item.ev == 0);
            int class2Success = correctClass2 / data.Count(item => item.ev == 1);

            return 50 * (class1Success + class2Success);
        }
    }
}
