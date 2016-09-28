using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TaxEngine
{
    internal enum ContextStatus
    {
        Uninitialized,
        ExpParsing,
        ExpParsed,
        Exp1Parsing,
        Exp1Parsed,
        RPNParsing,
        RPNParsed,
        Calculating,
        Calculated
    }

    internal enum VariableStatus
    {
        Exp1Parsing,
        Exp1Parsed
    }
}
