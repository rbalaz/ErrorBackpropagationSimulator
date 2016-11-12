using System;

namespace ErrorBackpropagationMVC
{
    public class ActivationFunction
    {
        public double alfa { get; private set; }

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
            return (alfa * Math.Exp(-alfa * x)) / ((1 + Math.Exp(-alfa * x)) * (1 + Math.Exp(-alfa * x)));
        }
    }
}