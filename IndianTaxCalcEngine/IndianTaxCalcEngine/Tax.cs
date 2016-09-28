using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IndianTaxCalcEngine
{
    public class Tax
    {
        public string TaxCode { get; private set; }
        public decimal TaxRate { get; private set; }
        public TaxBasisType TaxBasisType { get; private set; }
        public bool IsPriceInclTax { get; private set; }
        public decimal Factor { get; private set; }
        public string CalculationFormula { get; private set; }

        private readonly ITaxBasisCalculation Calculation;
        private readonly TaxCalculationContext Context;

        public Tax(string code, decimal rate, TaxBasisType basistype, bool isPriceIncl, decimal factor, string formula, TaxCalculationContext context)
        {
            TaxCode = code;
            TaxRate = rate;
            TaxBasisType = basistype;
            IsPriceInclTax = isPriceIncl;
            Factor = factor;
            CalculationFormula = formula;

            context.Taxs[TaxCode] = this;
            Context = context;
            Calculation = this.CreateTaxBasisCalculation(TaxBasisType);
        }

        public Tax(string code, decimal rate, TaxBasisType basistype, bool isPriceIncl, decimal factor, TaxCalculationContext context)
            : this(code, rate, basistype, isPriceIncl, factor, string.Empty, context)
        {
        }

        public decimal TaxBasisValue
        {
            get
            {
                return 0M;
            }
        }

        public decimal Percent
        {
            get
            {
                return 0M;
            }
        }

        public decimal TaxValue
        {
            get
            {
                return this.TaxBasisValue * this.Percent;
            }
        }

        public ITaxBasisCalculation CreateTaxBasisCalculation(TaxBasisType basistype)
        {
            switch (basistype)
            {
                case TaxBasisType.Line:
                    return new LineTaxBasisCalculation(this, this.Context);
                case TaxBasisType.Asses:
                    return new AssesTaxBasisCalculation(this, this.Context);
                case TaxBasisType.EXLine:
                    return new EXLineTaxBasisCalculation(this, this.Context);
                case TaxBasisType.MRP:
                    return new MRPTaxBasisCalculation(this, this.Context);
            }
            return null;
        }
    }
}
