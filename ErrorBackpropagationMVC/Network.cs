using System;
using System.Collections.Generic;

namespace ErrorBackpropagationMVC
{
    public class Network
    {
        public List<Neuron[]> layers { get; private set; }
        public bool currentIterationSuccess { get; private set; }

        public double currentIterationError { get; private set; }

        public double learningParameter { get; set; }

        public double outputTolerance { get; set; }

        public Network()
        {
            layers = new List<Neuron[]>();
        }

        public void addLayer(Neuron[] layer)
        {
            layers.Add(layer);
        }

        public double compute(Data data)
        {
            currentIterationSuccess = false;
            for (int i = 0; i < layers[0].Length; i++)
            {
                layers[0][i].evaluateInput(data);
                layers[0][i].evaluateOutput();
            }
            return compute(layers[1], data);
        }

        public double compute(Neuron[] layer, Data data)
        {
            for (int i = 0; i < layer.Length; i++)
            {
                List<double> previousLayerOutputsForSpecificNeuron = new List<double>();
                foreach (Synapse s in layer[i].synapses)
                {
                    if (s.beforeNeuron != null)
                        previousLayerOutputsForSpecificNeuron.Add(s.getOutputFromLeftNeuron());
                }
                layer[i].evaluateInput(previousLayerOutputsForSpecificNeuron.ToArray());
                layer[i].evaluateOutput();
            }
            if (layer[0].type == Neuron.NeuronTypes.output)
            {
                for (int i = 0; i < layer.Length; i++)
                    layer[i].applyThreshold(outputTolerance);

                string binaryEv = Convert.ToString(data.ev, 2);
                while (binaryEv.Length < layer.Length)
                    binaryEv = string.Concat("0", binaryEv);

                currentIterationSuccess = true;
                for (int i = 0; i < layer.Length; i++)
                {
                    if (layer[i].thresholdedOutput != char.GetNumericValue(binaryEv[i]))
                        currentIterationSuccess = false;
                }

                if (currentIterationSuccess)
                    return data.ev;
                else
                    return -1;
            }
            else
                return compute(layers[layers.IndexOf(layer) + 1], data);
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

        private void propagate(Neuron[] layer, Data data)
        {
            for (int i = 0; i < layer.Length; i++)
            {
                List<double> previousLayerOutputsForSpecificNeuron = new List<double>();
                foreach (Synapse s in layer[i].synapses)
                {
                    if (s.beforeNeuron != null)
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
                string binaryEv = Convert.ToString(data.ev, 2);
                while (binaryEv.Length < layer.Length)
                    binaryEv = string.Concat("0", binaryEv);
                currentIterationError = 0;
                currentIterationSuccess = true;
                for (int i = 0; i < layer.Length; i++)
                {
                    layer[i].evaluateErrorSignal(char.GetNumericValue(binaryEv[i]) - layer[i].output);
                    currentIterationError += char.GetNumericValue(binaryEv[i]) - layer[i].output;
                    if (Math.Abs(char.GetNumericValue(binaryEv[i]) - layer[i].output) > outputTolerance)
                        currentIterationSuccess = false;
                }
                backPropagate(layers[layers.IndexOf(layer) - 1], data);
            }
            else
            {
                for (int i = 0; i < layer.Length; i++)
                {
                    double value = 0;
                    for (int j = 0; j < layers[layers.IndexOf(layer) + 1].Length; j++)
                    {
                        double errorSignal = layers[layers.IndexOf(layer) + 1][j].errorSignal;
                        double weight = layers[layers.IndexOf(layer) + 1][j].synapses.Find(item => item.beforeNeuron == layer[i]).getWeight();
                        value += errorSignal * weight;
                    }
                    layer[i].evaluateErrorSignal(value);
                }
                if (layer[0].type == Neuron.NeuronTypes.input)
                    changeCurrentWeights(layer, data);
                else
                    backPropagate(layers[layers.IndexOf(layer) - 1], data);
            }
        }

        private void changeCurrentWeights(Neuron[] layer, Data data)
        {
            for (int i = 0; i < layer.Length; i++)
            {
                int inputCounter = 0;
                foreach (Synapse s in layer[i].synapses)
                {
                    try
                    {
                        s.incrementWeight(learningParameter * layer[i].errorSignal * data.getInputByNumber(inputCounter++));
                    }
                    catch (Data.IllegalArgumentException)
                    {
                        s.incrementWeight(learningParameter * layer[i].errorSignal * layer[i].bias);
                    }
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
                    try
                    {
                        s.incrementWeight(learningParameter * s.afterNeuron.errorSignal * s.beforeNeuron.output);
                    }
                    catch (NullReferenceException)
                    {
                        s.incrementWeight(learningParameter * s.afterNeuron.errorSignal * s.afterNeuron.bias);
                    }
                }
            }
            if (layer[0].type == Neuron.NeuronTypes.hidden)
                changeCurrentWeights(layers[layers.IndexOf(layer) + 1]);
        }
    }
}