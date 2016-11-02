using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrorBackpropagationSimulator
{
    class Initialiser
    {
        private string initString;
        private int inputs;

        public Initialiser(string initString, int inputs)
        {
            this.initString = initString;
            this.inputs = inputs;
        }

        public Initialiser() { }

        public Network createNeuralNetwork()
        {
            string[] stringNeurons = initString.Split(' ');
            int[] neurons = new int[stringNeurons.Length];
            for (int i = 0; i < stringNeurons.Length; i++)
            {
                neurons[i] = int.Parse(stringNeurons[i]);
            }

            Neuron[] inputLayer = new Neuron[neurons[0]];
            Neuron[] hiddenLayer1 = new Neuron[neurons[1]];
            Neuron[] hiddenLayer2 = new Neuron[neurons[2]];
            Neuron[] outputLayer = new Neuron[neurons[3]];

            fillLayer(inputLayer, inputs, Neuron.NeuronTypes.input);
            // fixed synapses not being properly initiated for non-input neurons
            fillNonInputLayer(inputLayer, hiddenLayer1, inputLayer.Length, Neuron.NeuronTypes.hidden);
            fillNonInputLayer(hiddenLayer1, hiddenLayer2, hiddenLayer1.Length, Neuron.NeuronTypes.hidden);
            fillNonInputLayer(hiddenLayer2, outputLayer, hiddenLayer2.Length, Neuron.NeuronTypes.output);

            Network network = new Network();
            network.addLayer(inputLayer);
            network.addLayer(hiddenLayer1);
            network.addLayer(hiddenLayer2);
            network.addLayer(outputLayer);

            return network;
        }

        public Network createMultiHiddenLayerNetwork()
        {
            string[] stringNeurons = initString.Split(' ');
            int[] neurons = new int[stringNeurons.Length];
            for (int i = 0; i < stringNeurons.Length; i++)
            {
                neurons[i] = int.Parse(stringNeurons[i]);
            }

            Neuron[] inputLayer = new Neuron[neurons[0]];
            Neuron[][] hiddenLayers = new Neuron[stringNeurons.Length - 2][];
            for (int i = 0; i < stringNeurons.Length - 2; i++)
            {
                hiddenLayers[i] = new Neuron[neurons[1 + i]];
            }
            Neuron[] outputLayer = new Neuron[neurons[neurons.Length - 1]];

            fillLayer(inputLayer, inputs, Neuron.NeuronTypes.input);
            // fixed synapses not being properly initiated for non-input neurons
            fillNonInputLayer(inputLayer, hiddenLayers[0], inputLayer.Length, Neuron.NeuronTypes.hidden);
            for (int i = 1; i < stringNeurons.Length - 2; i++)
            {
                fillNonInputLayer(hiddenLayers[i - 1], hiddenLayers[i], hiddenLayers[i - 1].Length, Neuron.NeuronTypes.hidden);
            }
            fillNonInputLayer(hiddenLayers[stringNeurons.Length - 3], outputLayer, hiddenLayers[stringNeurons.Length - 3].Length, Neuron.NeuronTypes.output);

            Network network = new Network();
            network.addLayer(inputLayer);
            for (int i = 0; i < stringNeurons.Length - 2; i++)
            {
                network.addLayer(hiddenLayers[i]);
            }
            network.addLayer(outputLayer);

            return network;
        }

        // Weight initialising changed from 0 to 1
        private void fillLayer(Neuron[] layer, int inputs, Neuron.NeuronTypes type)
        {
            for (int i = 0; i < layer.Length; i++)
            {
                layer[i] = new Neuron(type, 1, -1);
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
                biasSynapse.setWeight(weights[layer.Length][inputs]);
                layer[i].synapses.Add(biasSynapse);
            }
        }

        private void fillNonInputLayer(Neuron[] previousLayer, Neuron[] layer, int inputs, Neuron.NeuronTypes type)
        {
            for (int i = 0; i < layer.Length; i++)
            {
                layer[i] = new Neuron(type, 1, -1);
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
                biasSynapse.setWeight(weights[layer.Length][inputs]);
                layer[i].synapses.Add(biasSynapse);
            }
        }

        public Network createCV5Network()
        {
            Neuron[] inputLayer = new Neuron[2];
            Neuron[] hiddenLayer = new Neuron[2];
            Neuron[] outputLayer = new Neuron[1];

            for (int i = 0; i < inputLayer.Length; i++)
            {
                inputLayer[i] = new Neuron(Neuron.NeuronTypes.input, 0.75, -1);
                Synapse wx1 = new Synapse(inputLayer[i]);
                wx1.setWeight(0.6);
                inputLayer[i].addSynapse(wx1);
                Synapse wx2 = new Synapse(inputLayer[i]);
                wx2.setWeight(0.4);
                inputLayer[i].addSynapse(wx2);
                Synapse wx3 = new Synapse(inputLayer[i]);
                wx3.setWeight(0.9);
                inputLayer[i].addSynapse(wx3);
                Synapse bias = new Synapse(inputLayer[i]);
                bias.setWeight(1);
                inputLayer[i].addSynapse(bias);
            }

            for (int i = 0; i < hiddenLayer.Length; i++)
            {
                hiddenLayer[i] = new Neuron(Neuron.NeuronTypes.hidden, 0.75, -1);
                Synapse w1 = new Synapse(inputLayer[0], hiddenLayer[i]);
                w1.setWeight(-0.3);
                hiddenLayer[i].addSynapse(w1);
                Synapse w2 = new Synapse(inputLayer[1], hiddenLayer[i]);
                w2.setWeight(0.3);
                hiddenLayer[i].addSynapse(w2);
                Synapse bias = new Synapse(hiddenLayer[i]);
                bias.setWeight(1);
                hiddenLayer[i].addSynapse(bias);
            }

            for (int i = 0; i < outputLayer.Length; i++)
            {
                outputLayer[i] = new Neuron(Neuron.NeuronTypes.output, 0.75, -1);
                Synapse w3 = new Synapse(hiddenLayer[0], outputLayer[i]);
                w3.setWeight(0.245);
                outputLayer[i].addSynapse(w3);
                Synapse w4 = new Synapse(hiddenLayer[1], outputLayer[i]);
                w4.setWeight(0.55);
                outputLayer[i].addSynapse(w4);
                Synapse bias = new Synapse(outputLayer[i]);
                bias.setWeight(1);
                outputLayer[i].addSynapse(bias);
            }

            Network network = new Network();
            network.addLayer(inputLayer);
            network.addLayer(hiddenLayer);
            network.addLayer(outputLayer);

            return network;
        }

        private double generateSynapseWeight(Neuron[] previousLayer, int inputs)
        {
            if (previousLayer == null)
            {
                Random random = new Random();
                int intWeight = random.Next(48);
                return ((intWeight) - 24) / (10 * inputs);
            }
            else
            {
                Random random = new Random();
                int intWeight = random.Next(48);
                return ((intWeight) - 24) / (10 * previousLayer.Length);
            }
        }

        public Network loadNetworkFromFile(string fileName, out Data[] trainingData, out Data[] testingData)
        {
            FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            StreamReader reader = new StreamReader(stream);
            string line = reader.ReadLine();
            string[] neuronsInLayers = line.Split();
            string activationParameters = reader.ReadLine();
            double alfa = double.Parse(activationParameters.Split(' ')[1]);
            double learningParameter = double.Parse(reader.ReadLine());
            double outputTolerance = double.Parse(reader.ReadLine());

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
                }
                if (line.Contains("Neuron"))
                    neuronCounter++;

                weights[layerCounter][neuronCounter] = Array.ConvertAll(line.Split(' '), s => double.Parse(s, System.Globalization.CultureInfo.InvariantCulture));
            }

            List<Data> trainingDataList = new List<Data>();
            List<Data> testingDataList = new List<Data>();
            while ((line = reader.ReadLine()) != "Testing data")
            {
                double[] inputs = Array.ConvertAll(line.Split(' '), s => double.Parse(s, System.Globalization.CultureInfo.InvariantCulture));
                trainingDataList.Add(new Data(inputs));
            }
            while ((line = reader.ReadLine()) != null)
            {
                double[] inputs = Array.ConvertAll(line.Split(' '), s => double.Parse(s, System.Globalization.CultureInfo.InvariantCulture));
                testingDataList.Add(new Data(inputs));
            }
            trainingData = trainingDataList.ToArray();
            testingData = testingDataList.ToArray();

            fillLayer(layers[0], weights[0][0].Length, Neuron.NeuronTypes.input, weights[0], alfa);
            for (int i = 1; i < neuronsInLayers.Length - 2; i++)
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
