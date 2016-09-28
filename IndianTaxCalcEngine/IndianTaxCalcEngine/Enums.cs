using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IndianTaxCalcEngine
{
    public enum TaxBasisType
    {
        Line,
        Asses,
        EXLine,
        MRP
    }
    public enum Direction
    {
        AR,
        AP
    }

    internal enum Status
    {
        NotStart,
        Preparing,
        Prepared,
        Calculating,
        Calculated
    }
}
