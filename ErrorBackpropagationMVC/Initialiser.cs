using System;
using System.IO;
using System.Collections.Generic;
using System.Threading;
using System.Globalization;

namespace ErrorBackpropagationMVC
{
    public class Initialiser
    {
        private string initString;
        private int inputs;
        private int outputs;
        private double alfa;

        public Initialiser(string initString, int inputs, double alfa, int outputs)
        {
            this.initString = initString;
            this.inputs = inputs;
            this.outputs = outputs;
            this.alfa = alfa;
        }

        public Initialiser() { }

        public Network createMultiHiddenLayerNetwork()
        {
            string[] stringNeurons = initString.Split(' ');
            int[] neurons = new int[stringNeurons.Length + 1];
            for (int i = 0; i < stringNeurons.Length; i++)
            {
                neurons[i] = int.Parse(stringNeurons[i]);
            }
            if (outputs == 2)
                neurons[stringNeurons.Length] = 1;
            else
                neurons[stringNeurons.Length] = 2;

            Neuron[] inputLayer = new Neuron[neurons[0]];
            Neuron[][] hiddenLayers = new Neuron[stringNeurons.Length - 1][];
            for (int i = 0; i < stringNeurons.Length - 1; i++)
            {
                hiddenLayers[i] = new Neuron[neurons[1 + i]];
            }
            Neuron[] outputLayer = new Neuron[neurons[neurons.Length - 1]];

            fillLayer(inputLayer, inputs, Neuron.NeuronTypes.input, alfa);
            fillNonInputLayer(inputLayer, hiddenLayers[0], inputLayer.Length, Neuron.NeuronTypes.hidden, alfa);
            for (int i = 1; i < stringNeurons.Length - 1; i++)
            {
                fillNonInputLayer(hiddenLayers[i - 1], hiddenLayers[i], hiddenLayers[i - 1].Length, Neuron.NeuronTypes.hidden, alfa);
            }
            fillNonInputLayer(hiddenLayers[stringNeurons.Length - 2], outputLayer, hiddenLayers[stringNeurons.Length - 2].Length, Neuron.NeuronTypes.output, alfa);

            Network network = new Network();
            network.addLayer(inputLayer);
            for (int i = 0; i < stringNeurons.Length - 1; i++)
            {
                network.addLayer(hiddenLayers[i]);
            }
            network.addLayer(outputLayer);

            return network;
        }

        private void fillLayer(Neuron[] layer, int inputs, Neuron.NeuronTypes type, double alfa)
        {
            for (int i = 0; i < layer.Length; i++)
            {
                layer[i] = new Neuron(type, alfa, -1);
                for (int j = 0; j < inputs; j++)
                {
                    Synapse s = new Synapse(layer[i]);
                    s.setWeight(generateSynapseWeight(null, inputs));
                    layer[i].synapses.Add(s);
                }
                Synapse biasSynapse = new Synapse(layer[i]);
                biasSynapse.setWeight(1);
                layer[i].synapses.Add(biasSynapse);
            }
        }

        private void fillLayer(Neuron[] layer, int inputs, Neuron.NeuronTypes type, double[][] weights, double alfa)
        {
            for (int i = 0; i < layer.Length; i++)
            {
                layer[i] = new Neuron(type, alfa, -1);
                for (int j = 0; j < inputs; j++)
                {
                    Synapse s = new Synapse(layer[i]);
                    s.setWeight(weights[i][j]);
                    layer[i].synapses.Add(s);
                }
                Synapse biasSynapse = new Synapse(layer[i]);
                biasSynapse.setWeight(weights[i][inputs]);
                layer[i].synapses.Add(biasSynapse);
            }
        }

        private void fillNonInputLayer(Neuron[] previousLayer, Neuron[] layer, int inputs, Neuron.NeuronTypes type, double alfa)
        {
            for (int i = 0; i < layer.Length; i++)
            {
                layer[i] = new Neuron(type, alfa, -1);
                for (int j = 0; j < inputs; j++)
                {
                    Synapse s = new Synapse(previousLayer[j], layer[i]);
                    s.setWeight(generateSynapseWeight(previousLayer, inputs));
                    layer[i].synapses.Add(s);
                }
                Synapse biasSynapse = new Synapse(layer[i]);
                biasSynapse.setWeight(1);
                layer[i].synapses.Add(biasSynapse);
            }
        }

        private void fillNonInputLayer(Neuron[] previousLayer, Neuron[] layer, int inputs, Neuron.NeuronTypes type, double[][] weights, double alfa)
        {
            for (int i = 0; i < layer.Length; i++)
            {
                layer[i] = new Neuron(type, alfa, -1);
                for (int j = 0; j < inputs; j++)
                {
                    Synapse s = new Synapse(previousLayer[j], layer[i]);
                    s.setWeight(weights[i][j]);
                    layer[i].synapses.Add(s);
                }
                Synapse biasSynapse = new Synapse(layer[i]);
                biasSynapse.setWeight(weights[i][inputs]);
                layer[i].synapses.Add(biasSynapse);
            }
        }

        private double generateSynapseWeight(Neuron[] previousLayer, int inputs)
        {
            Thread.Sleep(10);
            double weight;
            if (previousLayer == null)
            {
                Random random = new Random();
                int intWeight = random.Next(48);
                weight = (double)(intWeight - 24) / (double)(10 * inputs);
            }
            else
            {
                Random random = new Random();
                int intWeight = random.Next(48);
                weight = (double)(intWeight - 24) / (double)(10 * previousLayer.Length);
            }
            return weight;
        }

        public Network loadNetworkFromFile(string fileName, out Data[] trainingData, out Data[] testingData)
        {
            FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            StreamReader reader = new StreamReader(stream);
            NumberFormatInfo format = new NumberFormatInfo();
            format.NumberDecimalSeparator = ".";

            string line = reader.ReadLine();
            string[] neuronsInLayers = line.Split(' ');
            string activationParameters = reader.ReadLine();
            double alfa = double.Parse(activationParameters);
            double learningParameter = double.Parse(reader.ReadLine(), format);
            double outputTolerance = double.Parse(reader.ReadLine(), format);

            Neuron[][] layers = new Neuron[neuronsInLayers.Length][];
            double[][][] weights = new double[neuronsInLayers.Length][][];
            for (int i = 0; i < neuronsInLayers.Length; i++)
            {
                layers[i] = new Neuron[int.Parse(neuronsInLayers[i])];
                weights[i] = new double[int.Parse(neuronsInLayers[i])][];
            }


            int neuronCounter = 0;
            int layerCounter = 0;

            while ((line = reader.ReadLine()) != "Training data")
            {
                if (line.Contains("Layer"))
                {
                    layerCounter++;
                    neuronCounter = 0;
                    continue;
                }
                if (line.Contains("Neuron"))
                {
                    neuronCounter++;
                    continue;
                }

                weights[layerCounter - 1][neuronCounter - 1] = Array.ConvertAll(line.Split(' '), s => double.Parse(s, format));
            }

            List<Data> trainingDataList = new List<Data>();
            List<Data> testingDataList = new List<Data>();
            while ((line = reader.ReadLine()) != "Testing data")
            {
                double[] inputs = Array.ConvertAll(line.Split(' '), s => double.Parse(s, format));
                trainingDataList.Add(new Data(inputs));
            }
            while ((line = reader.ReadLine()) != null)
            {
                double[] inputs = Array.ConvertAll(line.Split(' '), s => double.Parse(s, format));
                testingDataList.Add(new Data(inputs));
            }
            trainingData = trainingDataList.ToArray();
            testingData = testingDataList.ToArray();

            fillLayer(layers[0], weights[0][0].Length - 1, Neuron.NeuronTypes.input, weights[0], alfa);
            for (int i = 1; i < neuronsInLayers.Length - 1; i++)
            {
                fillNonInputLayer(layers[i - 1], layers[i], layers[i - 1].Length, Neuron.NeuronTypes.hidden, weights[i], alfa);
            }
            fillNonInputLayer(layers[neuronsInLayers.Length - 2], layers[neuronsInLayers.Length - 1],
                layers[neuronsInLayers.Length - 2].Length, Neuron.NeuronTypes.output, weights[neuronsInLayers.Length - 1], alfa);

            Network network = new Network();
            for (int i = 0; i < neuronsInLayers.Length; i++)
            {
                network.addLayer(layers[i]);
            }
            network.learningParameter = learningParameter;
            network.outputTolerance = outputTolerance;

            return network;
        }
    }
}
