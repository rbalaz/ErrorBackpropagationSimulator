﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrorBackpropagationSimulator
{
    class Data
    {
        public class IllegalArgumentException : Exception { }
        public double x1 { get; private set; }
        public double x2 { get; private set; }
        public double x3 { get; private set; }
        public double x4 { get; private set; }
        public double x5 { get; private set; }
        public double x6 { get; private set; }
        public double x7 { get; private set; }
        public double x8 { get; private set; }
        public double x9 { get; private set; }
        public int ev { get; private set; }

        private int amount;

        public Data(double[] inputs)
        {
            if (inputs.Length == 10)
            {
                x1 = inputs[0];
                x2 = inputs[1];
                x3 = inputs[2];
                x4 = inputs[3];
                x5 = inputs[4];
                x6 = inputs[5];
                x7 = inputs[6];
                x8 = inputs[7];
                x9 = inputs[8];
                ev = (int)inputs[9];
                amount = 10;
            }
            if (inputs.Length == 4)
            {
                x1 = inputs[0];
                x2 = inputs[1];
                x3 = inputs[2];
                ev = (int)inputs[3];
                amount = 4;
            }
        }

        public double getInputByNumber(int input)
        {
            if (amount == 10)
            {
                switch (input)
                {
                    case 1: return x1;
                    case 2: return x2;
                    case 3: return x3;
                    case 4: return x4;
                    case 5: return x5;
                    case 6: return x6;
                    case 7: return x7;
                    case 8: return x8;
                    case 9: return x9;
                    default: throw new IllegalArgumentException();
                }
            }
            else
            {
                switch (input)
                {
                    case 1: return x1;
                    case 2: return x2;
                    case 3: return x3;
                    default: throw new IllegalArgumentException();
                }
            }
        }

        public override string ToString()
        {
            if (amount == 4)
                return (x1 + " " + x2 + " " + x3 + " " + ev);
            else
                return (x1 + " " + x2 + " " + x3 + " " + x4 + " " + x5 + " " + x6 + " " + x7 + " " + x8 + " " + x9 + " " + ev);
        }
    }
}
