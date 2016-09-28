using System;
using System.Collections.Generic;
using System.Text;
using YiLu.Components;
using YiLu.Logic;
using YiLu.Components.Resistor;
using YiLu.Components.LED;

namespace YiLu.Circuit
{
    public class SeriesCircuit
    {
        public SeriesCircuit(double invol, double outvol)
        {
            volin = invol;
            volout = outvol;
            leds = new List<LED>();
            protectResistor = new Resistor();
        }

        private double volin = 0.0;
        public double VoltageIn
        {
            get { return volin; }
            set { volin = value; }
        }

        private double volout = 0.0;
        public double VoltageOut
        {
            get { return volout; }
            set { volout = value; }
        }

        private List<LED> leds;
        public List<LED> LEDs
        {
            get { return leds; }
        }

        private Resistor protectResistor;
        public Resistor ProtectiveResistor
        {
            get
            {
                Refresh();
                return protectResistor;
            }
        }

        public double WorkCurrent()
        {
            double allcur = 0.0;
            foreach (LED led in leds)
            {
                allcur += led.WorkCurrent;
            }

            return (double)allcur / (double)leds.Count;
        }

        private void Refresh()
        {

            double allcur = 0.0;
            double allvf = 0.0;

            foreach (LED led in leds)
            {
                allcur += led.WorkCurrent;
                allvf += led.VoltageFall;
            }

            double avgcur = (double)allcur / (double)leds.Count;

            this.protectResistor.UpdateResistance(volin - volout - allvf, avgcur);
        }
    }
}
