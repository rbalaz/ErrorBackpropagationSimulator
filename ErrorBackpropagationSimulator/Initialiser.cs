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
            fillLayer(hiddenLayer1, inputLayer.Length, Neuron.NeuronTypes.hidden);
            fillLayer(hiddenLayer2, hiddenLayer1.Length, Neuron.NeuronTypes.hidden);
            fillLayer(outputLayer, hiddenLayer2.Length, Neuron.NeuronTypes.output);

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
                    s.weight = 0;
                    layer[i].synapses.Add(s);
                }
                Synapse biasSynapse = new Synapse(layer[i]);
                biasSynapse.weight = 1;
                layer[i].synapses.Add(biasSynapse);
            }
        }
    }
}
