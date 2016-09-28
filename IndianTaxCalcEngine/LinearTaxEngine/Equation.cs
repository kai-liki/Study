using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TaxEngine
{
    internal class Equation
    {
        public string Exp { get; set; }
        public string LExp { get; set; }
        public string RExp { get; set; }
        public string LRPN { get; set; }
        public string RRPN { get; set; }

        public string[] LeftTokens { get; set; }
        public string[] RightTokens { get; set; }

        public decimal Item0 { get; set; }
        public decimal Item1 { get; set; }

        public decimal GetX()
        {
            
            if (Item1 == 0)
            {
                throw new DivideByZeroException(string.Format("Item0: {0} Item1: {1}", Item0, Item1));
            }
            else
            {
                return -1 * Item0 / Item1;
            }
        }
    }
}
