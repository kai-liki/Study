using System;
using System.Collections.Generic;
using System.Text;
using YiLu.Logic;

namespace YiLu.Components.Resistor
{
    public class Resistor
    {
        public Resistor()
        {
        }

        public double Power
        {
            get { return 0.0; }//Analog.GetPower(ElectricCurrent, resistance); }
        }

        public void UpdateResistance(double v, double i)
        {
            resistance = Analog.GetResistance(v, i);
        }

        private double resistance = double.MaxValue;
        public double Resistance
        {
            get
            {
                return resistance;
            }
        }
    }
}
