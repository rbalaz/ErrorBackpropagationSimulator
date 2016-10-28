using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrorBackpropagationSimulator
{
    class Network
    {
        public List<Neuron[]> layers { get; private set; }
        public bool currentIterationSuccess { get; private set; }

        public double learningParameter { get; set; }

        public Network()
        {
            layers = new List<Neuron[]>();
        }

        public void addLayer(Neuron[] layer)
        {
            layers.Add(layer);
        }

        public void propagate(Data data)
        {
            currentIterationSuccess = false;
            for (int i = 0; i < layers[0].Length; i++)
            {
                layers[0][i].evaluateInput(data);
                layers[0][i].evaluateOutput();
            }
            propagate(layers[1], data);
        }

        public double compute(Data data)
        {
            for (int i = 0; i < layers[0].Length; i++)
            {
                layers[0][i].evaluateInput(data);
                layers[0][i].evaluateOutput();
            }
            return compute(layers[1]);
        }

        public double compute(Neuron[] layer)
        {
            for (int i = 0; i < layer.Length; i++)
            {
                List<double> previousLayerOutputsForSpecificNeuron = new List<double>();
                foreach (Synapse s in layer[i].synapses)
                {
                    previousLayerOutputsForSpecificNeuron.Add(s.getOutputFromLeftNeuron());
                }
                layer[i].evaluateInput(previousLayerOutputsForSpecificNeuron.ToArray());
                layer[i].evaluateOutput();
            }
            if (layer[0].type == Neuron.NeuronTypes.output)
                return layer[0].output;
            else
                return compute(layers[layers.IndexOf(layer) + 1]);
        }

        private void propagate(Neuron[] layer, Data data)
        {
            for (int i = 0; i < layer.Length; i++)
            {
                List<double> previousLayerOutputsForSpecificNeuron = new List<double>();
                foreach (Synapse s in layer[i].synapses)
                {
                    previousLayerOutputsForSpecificNeuron.Add(s.getOutputFromLeftNeuron());
                }
                layer[i].evaluateInput(previousLayerOutputsForSpecificNeuron.ToArray());
                layer[i].evaluateOutput();
            }
            if (layer[0].type == Neuron.NeuronTypes.output)
                backPropagate(layer, data);
            else
                propagate(layers[layers.IndexOf(layer) + 1], data);
        }

        private void backPropagate(Neuron[] layer, Data data)
        {
            if (layer[0].type == Neuron.NeuronTypes.output)
            {
                if (data.ev != layer[0].output)
                {
                    layer[0].evaluateErrorSignal(data.ev - layer[0].output);
                    backPropagate(layers[layers.IndexOf(layer) - 1], data);
                }
                else
                    currentIterationSuccess = true;
            }
            else
            {
                for (int i = 0; i < layer.Length; i++)
                {
                    double value = 0;
                    for (int j = 0; j < layers[layers.IndexOf(layer) + 1].Length; j++)
                    {
                        double errorSignal = layers[layers.IndexOf(layer) + 1][j].errorSignal;
                        double weight = layers[layers.IndexOf(layer) + 1][j].synapses.Find(item => item.beforeNeuron == layer[i]).weight;
                        value += errorSignal * weight;
                    }
                    layer[i].evaluateErrorSignal(value);
                }
                if (layer[0].type == Neuron.NeuronTypes.input)
                    changeCurrentWeights(layer, data);
            }
        }

        private void changeCurrentWeights(Neuron[] layer, Data data)
        {
            for (int i = 0; i < layer.Length; i++)
            {
                int inputCounter = 1;
                foreach (Synapse s in layer[i].synapses)
                {
                    s.weight = learningParameter * layer[i].errorSignal * data.getInputByNumber(inputCounter++);
                }
            }
            changeCurrentWeights(layers[layers.IndexOf(layer) + 1]);
        }

        private void changeCurrentWeights(Neuron[] layer)
        {
            for (int i = 0; i < layer.Length; i++)
            {
                foreach (Synapse s in layer[i].synapses)
                {
                    s.weight = learningParameter * s.afterNeuron.errorSignal * s.beforeNeuron.output;
                }
            }
            if (layer[0].type == Neuron.NeuronTypes.hidden)
                changeCurrentWeights(layers[layers.IndexOf(layer) + 1]);
        }
    }
}
