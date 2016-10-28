using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrorBackpropagationSimulator
{
    class Synapse
    {
        public double weight
        {
            get
            {
                return weight;
            }
            set
            {
                weight += value;
            }
        }
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

    }
}
