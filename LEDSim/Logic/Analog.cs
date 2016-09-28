using System;
using System.Collections.Generic;
using System.Text;

namespace YiLu.Logic
{
    public class Analog
    {
        public class ShortedException : Exception
        {
            public override string Message
            {
                get
                {
                    return "Short occur. Resistance is extremely small or equals to zero.";
                }
            }
        }

        public const double MINIUM_POSITIVE_DOUBLE = 10.0 * double.Epsilon;

        //已知电压电阻求电流
        static public double GetCurrent(double Voltage, double Resistance)
        {
            if (Voltage < MINIUM_POSITIVE_DOUBLE)
                return 0.0;
            if (Resistance < MINIUM_POSITIVE_DOUBLE)
                return double.PositiveInfinity;
            return (double)(Voltage / Resistance);
        }

        //已知电流电阻求电压
        static public double GetVoltage(double Current, double Resistance)
        {
            return (double)(Current * Resistance);
        }

        //已知电压电流求电阻
        static public double GetResistance(double Voltage, double Current)
        {
            if (Voltage < MINIUM_POSITIVE_DOUBLE || Current < MINIUM_POSITIVE_DOUBLE)
                throw new ArgumentException("Invalid voltage or current value.");
            return (double)(Voltage / Current);
        }

        //已知电流电阻求功率
        static public double GetPower(double Current, double Resistance)
        {
            return (double)(Current * Current * Resistance);
        }

        //已知电流电压求功率
        static public double GetPowerUI(double Current, double Voltage)
        {
            return (double)(Current * Voltage);
        }
    }
}
