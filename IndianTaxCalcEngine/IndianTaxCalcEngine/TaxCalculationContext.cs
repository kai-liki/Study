using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IndianTaxCalcEngine
{
    public class TaxCalculationContext
    {
        public decimal Price { get; set; }
        public decimal AssessableValue { get; set; }
        public decimal MRP { get; set; }
        public Dictionary<Tuple<string, Direction>,decimal> MiscCharges { get; private set; }
        public decimal AmountOrigin { get; set; }
        public decimal AllTaxBasis { get; set; }
        public bool IsMRP { get; set; }
        public Dictionary<string,Tax> Taxs { get; private set; }

        public TaxCalculationContext()
        {
            MiscCharges = new Dictionary<Tuple<string, Direction>, decimal>();
            Taxs = new Dictionary<string, Tax>();
            IsMRP = false;
        }
    }
}
