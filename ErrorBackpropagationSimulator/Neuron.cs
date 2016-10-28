using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrorBackpropagationSimulator
{
    class Neuron
    {
        class IllegalArgumentException : Exception { }
        public enum NeuronTypes { input, hidden, output }

        private ActivationFunction f;
        public double output { get; private set; }
        public double input { get; private set; }
        public double bias { get; private set; }
        public List<Synapse> synapses { get; }
        public NeuronTypes type { get; private set; }
        public double errorSignal { get; private set; }

        public Neuron(NeuronTypes type, double alfa, double bias)
        {
            this.type = type;
            this.bias = bias;
            synapses = new List<Synapse>();
            f = new ActivationFunction(alfa);
        }

        public void addSynapse(Synapse synapse)
        {
            synapses.Add(synapse);
        }

        public void evaluateInput(Data input)
        {
            this.input = 0;
            int inputCounter = 1;
            foreach (Synapse s in synapses)
            {
                try
                {
                    this.input += s.weight * input.getInputByNumber(inputCounter++);
                }
                catch (IllegalArgumentException)
                {
                    this.input += s.weight * bias;
                }
            }
        }
        public void evaluateInput(double[] input)
        {
            this.input = 0;
            for (int i = 0; i < input.Length; i++)
            {
                this.input += synapses[i].weight * input[i];
            }
            this.input = synapses[synapses.Count - 1].weight * bias;
        }

        public void evaluateOutput()
        {
            output = f.getValue(input);
        }

        private double evaluateDerivatedOutput()
        {
            return f.getDerivatedValue(input);
        }

        public void evaluateErrorSignal(double value)
        {
            errorSignal = value * evaluateDerivatedOutput();
        }
    }
}
