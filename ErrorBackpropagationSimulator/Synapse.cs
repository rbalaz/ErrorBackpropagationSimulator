using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrorBackpropagationSimulator
{
    class Synapse
    {
        private double weight;
        public Neuron beforeNeuron { get; private set; }
        public Neuron afterNeuron { get; set; }

        public Synapse(Neuron beforeNeuron, Neuron afterNeuron)
        {
            this.beforeNeuron = beforeNeuron;
            this.afterNeuron = afterNeuron;
        }

        public Synapse(Neuron afterNeuron)
        {
            this.afterNeuron = afterNeuron;
        }

        public double getOutputFromLeftNeuron()
        {
            return beforeNeuron.output;
        }

        public double getWeight()
        {
            return weight;
        }

        public void incrementWeight(double increment)
        {
            weight += increment;
        }

        public void setWeight(double value)
        {
            weight = value;
        }
    }
}
