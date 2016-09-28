using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IndianTaxCalcEngine
{
    public abstract class TaxBasisCalculation : ITaxBasisCalculation
    {
        public TaxBasisCalculation(Tax tax, TaxCalculationContext context)
        {
            Tax = tax;
            Context = context;
            CalculationStatus = Status.NotStart;
        }
        public abstract void CalculateTaxBasisPrepare();

        public abstract decimal CalculateTaxBasis(decimal amountOrigin);

        public abstract decimal CalculateTaxAmount();

        public abstract decimal GetLinearTerm();

        public abstract decimal GetConstantTerm();

        public Tax Tax
        {
            get;
            private set;
        }

        public TaxCalculationContext Context
        {
            get;
            private set;
        }

        internal Status CalculationStatus
        {
            get;
            private set;
        }
    }
}
