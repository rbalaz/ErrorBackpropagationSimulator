using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrorBackpropagationMVC
{
    public class Neuron
    {
        public enum NeuronTypes { input, hidden, output }

        public ActivationFunction f { get; private set; }
        public double output { get; private set; }
        public double thresholdedOutput { get; private set; }
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

        public void evaluateInput(Data input)
        {
            this.input = 0;
            int inputCounter = 0;
            foreach (Synapse s in synapses)
            {
                try
                {
                    this.input += s.getWeight() * input.getInputByNumber(inputCounter++);
                }
                catch (Data.IllegalArgumentException)
                {
                    this.input += s.getWeight() * bias;
                }
            }
        }
        public void evaluateInput(double[] input)
        {
            this.input = 0;
            for (int i = 0; i < input.Length; i++)
            {
                this.input += synapses[i].getWeight() * input[i];
            }
            this.input += synapses[synapses.Count - 1].getWeight() * bias;
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

        public void applyThreshold(double tolerance)
        {
            if (output >= 0.5)
            {
                thresholdedOutput = output >= (1 - tolerance) ? 1 : output;
            }
            else
            {
                thresholdedOutput = output <= tolerance ? 0 : output;
            }
        }
    }
}
