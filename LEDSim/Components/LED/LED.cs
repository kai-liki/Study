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

        //ѹ��
        private double vfall = 0.0;
        public double VoltageFall
        {
            get { return vfall; }
        }

        //��������
        private double ecurrent = 0.0;
        public double WorkCurrent
        {
            get { return ecurrent; }
        }

        //ֱ����Ĭ��Ϊ3mm
        private int size = 3;
        public int Size
        {
            get { return size; }
        }

        //��ɫ
        private UInt32 color = 0xffffff;
        public uint Color
        {
            get { return color; }
        }

        //����
        public double Power
        {
            get { return Analog.GetPowerUI(ecurrent, vfall); }
        }
    }
}
