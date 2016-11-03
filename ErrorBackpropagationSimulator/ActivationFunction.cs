using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrorBackpropagationSimulator
{
    class ActivationFunction
    {
        public double alfa { get; private set; }
        public string type { get; private set; }

        public ActivationFunction(double alfa, string type)
        {
            this.alfa = alfa;
            this.type = type;
        }

        public double getValue(double x)
        {
            if (type.Equals("sigmoid"))
                return 1 / (1 + Math.Exp(-alfa * x));
            else
                return (Math.Exp(x) - Math.Exp(-x)) / (Math.Exp(x) + Math.Exp(-x));
        }

        // fixed activation function derivative being calculated incorrectly
        public double getDerivatedValue(double x)
        {
            if (type.Equals("sigmoid"))
                return (alfa * Math.Exp(-alfa * x)) / ((1 + Math.Exp(-alfa * x)) * (1 + Math.Exp(-alfa * x)));
            else
                return 1 - (getValue(x) * getValue(x));
        }
    }
}
