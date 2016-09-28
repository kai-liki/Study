using System;
using System.Collections.Generic;
using System.Text;
using YiLu.Logic;

namespace YiLu.Components.LED
{
    public class LED
    {
        public LED(double voltageFall, double workCurrent, int size, UInt32 color)
        {
            this.vfall = voltageFall;
            this.ecurrent = workCurrent;
            if (vfall < Analog.MINIUM_POSITIVE_DOUBLE || ecurrent < Analog.MINIUM_POSITIVE_DOUBLE)
                throw new ArgumentException("Invalid voltage or current value.");
            this.size = size;
            this.color = color;
        }

        //压降
        private double vfall = 0.0;
        public double VoltageFall
        {
            get { return vfall; }
        }

        //工作电流
        private double ecurrent = 0.0;
        public double WorkCurrent
        {
            get { return ecurrent; }
        }

        //直径，默认为3mm
        private int size = 3;
        public int Size
        {
            get { return size; }
        }

        //颜色
        private UInt32 color = 0xffffff;
        public uint Color
        {
            get { return color; }
        }

        //功率
        public double Power
        {
            get { return Analog.GetPowerUI(ecurrent, vfall); }
        }
    }
}
