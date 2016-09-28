using System;
using System.Collections.Generic;
using System.Text;
using YiLu.Components;
using YiLu.Logic;
using YiLu.Components.LED;

namespace YiLu.Circuit
{
    public class ParallelCircuit
    {
        public ParallelCircuit(double invol, double outvol)
        {
            volin = invol;
            volout = outvol;
            subSeries = new List<SeriesCircuit>(2);
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

        public void Refresh()
        {
        }

        private List<SeriesCircuit> subSeries;
        public List<SeriesCircuit> SubSeries
        {
            get { return subSeries; }
        }
    }
}
