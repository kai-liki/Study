using System;
using System.Collections.Generic;
using System.Text;
using YiLu.Components;

namespace YiLu.Circuit
{
    public interface I2PinCircuit
    {
        Pin InPin
        {
            get;
        }

        Pin OutPin
        {
            get;
        }

        bool Shorted
        {
            get;
        }

        void Refresh();
    }
}
