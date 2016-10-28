using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrorBackpropagationSimulator
{
    class ActivationFunction
    {
        private double alfa;

        public ActivationFunction(double alfa)
        {
            this.alfa = alfa;
        }

        public double getValue(double x)
        {
            return 1 / (1 + Math.Exp(-alfa * x));
        }

        public double getDerivatedValue(double x)
        {
            return (alfa * Math.Exp(-alfa * x)) /((1 + Math.Exp(-alfa * x) * (1 + Math.Exp(-alfa * x))));
        }
    }
}
