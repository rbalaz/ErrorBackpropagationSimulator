using System;
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

        private void fillLayer(Neuron[] layer, int inputs, Neuron.NeuronTypes type)
        {
            for (int i = 0; i < layer.Length; i++)
            {
                layer[i] = new Neuron(type, 0.75, -1);
                for (int j = 0; j < inputs; j++)
                {
                    Synapse s = new Synapse(layer[i]);
                    s.setWeight(0);
                    layer[i].synapses.Add(s);
                }
                Synapse biasSynapse = new Synapse(layer[i]);
                biasSynapse.setWeight(1);
                layer[i].synapses.Add(biasSynapse);
            }
        }

        private void fillNonInputLayer(Neuron[] previousLayer, Neuron[] layer, int inputs, Neuron.NeuronTypes type)
        {
            for (int i = 0; i < layer.Length; i++)
            {
                layer[i] = new Neuron(type, 0.75, -1);
                for (int j = 0; j < inputs; j++)
                {
                    Synapse s = new Synapse(previousLayer[j], layer[i]);
                    s.setWeight(0);
                    layer[i].synapses.Add(s);
                }
                Synapse biasSynapse = new Synapse(layer[i]);
                biasSynapse.setWeight(1);
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
    }
}
