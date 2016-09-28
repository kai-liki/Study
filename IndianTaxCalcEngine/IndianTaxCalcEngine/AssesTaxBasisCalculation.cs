using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IndianTaxCalcEngine
{
    public class AssesTaxBasisCalculation : TaxBasisCalculation
    {
        public AssesTaxBasisCalculation(Tax tax, TaxCalculationContext context)
            : base(tax, context)
        {
        }
        public override void CalculateTaxBasisPrepare()
        {
        }

        public override decimal CalculateTaxBasis(decimal amountOrigin)
        {
            return 0M;
        }

        public override decimal CalculateTaxAmount()
        {
            return 0M;
        }

        public override decimal GetLinearTerm()
        {
            return 0M;
        }

        public override decimal GetConstantTerm()
        {
            return 0M;
        }
    }
}
