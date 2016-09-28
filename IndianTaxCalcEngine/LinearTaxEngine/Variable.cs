using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TaxEngine
{
    internal class Variable
    {
        public string Exp { get; set; }
        public string Exp1 { get; set; }
        public string RPN { get; set; }

        public string[] Tokens { get; set; }

        public decimal GetVariableValue(decimal? x, string xname)
        {
            return Tokens.Calculate(x, xname);
        }
    }
}
