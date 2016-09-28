using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MS.Dynamics.Test.AxBusinessAbstractionLayer.India;
using MS.Dynamics.Test.AxBusinessAbstractionLayer.India.GeneralLedger;

namespace MS.Dynamics.TestTools.AxProxyLibrary.AxProxyHelpers.GLS.India.Tax
{
    public static class TaxCalculationHelper
    {
        public static TaxCalculationResult CalculateTax(ItemSalesTaxGroup_IN itg,decimal price, decimal assessval, decimal mrp, Direction direction, Dictionary<string,decimal> misccharges,Dictionary<string,decimal> taxcoderates)
        {
            Dictionary<string, decimal> newmisccharges = new Dictionary<string, decimal>();
            switch(direction)
            {
                case Direction.AP:
                    foreach (KeyValuePair<string, decimal> charge in misccharges)
                    {
                        newmisccharges[ConvertToAPChargeCode(charge.Key)] = charge.Value;
                    }
                    break;
                case Direction.AR:
                    foreach (KeyValuePair<string, decimal> charge in misccharges)
                    {
                        newmisccharges[ConvertToARChargeCode(charge.Key)] = charge.Value;
                    }
                    break;
            }
            TaxGroup tg = new TaxGroup(price, assessval, mrp, newmisccharges);
            foreach (ItemSalesTaxGroup_IN.FormulaDesignerIndirectTax t in itg.FormulaDesigner)
            {
                TaxBasisType basistype = TaxBasisType.Line;
                switch(t.TaxableBasis)
                {
                    case Enums.TaxableBasis_IN.LineAmount:
                        basistype = TaxBasisType.Line;
                        break;
                    case Enums.TaxableBasis_IN.Assessable:
                        basistype = TaxBasisType.ASSESS;
                        break;
                    case Enums.TaxableBasis_IN.MRP:
                        basistype = TaxBasisType.MRP;
                        break;
                    case Enums.TaxableBasis_IN.ExclAmount:
                        basistype = TaxBasisType.EXLine;
                        break;
                }
                decimal abatement = 0.0M;
                foreach (ItemSalesTaxGroup_IN.ItemSalesTaxGroupCode_IN tc in itg.ItemSalesTaxGroupCodes_IN)
                {
                    if (t.SalesTaxCode.Equals(tc.SalesTaxCode))
                    {
                        abatement = tc.AbatementPercent == null ? 0.0M : tc.AbatementPercent.Value / 100M;
                        break;
                    }
                }
                bool priceincl = t.PriceInclTax == null ? false : t.PriceInclTax.Value;
                Tax tax = new Tax(t.SalesTaxCode, taxcoderates[t.SalesTaxCode], basistype, priceincl, t.CalcExp1, abatement, tg);
            }

            return CalculateTax(tg);
        }

        private static TaxCalculationResult CalculateTax(TaxGroup tg)
        {
            decimal totaltax = tg.CalculateTaxes();

            TaxCalculationResult result = new TaxCalculationResult();
            result.TotalAmount = totaltax;
            int n=tg.Taxes.Count;
            result.TaxCodes = new string[n];
            result.AmountOrigin = new decimal[n];
            result.Percent = new decimal[n];
            result.CalcTaxAmount = new decimal[n];
            int i = 0;
            foreach (KeyValuePair<string, Tax> t in tg.Taxes)
            {
                Tax tax = t.Value;
                result.TaxCodes[i] = tax.Tax_Code;
                result.AmountOrigin[i] = tax.TaxBasisValue;
                result.Percent[i] = tax.Tax_Rate;
                result.CalcTaxAmount[i] = tax.TaxValue;
                i++;
            }
            return result;
        }

        internal static string ConvertToAPChargeCode(string calcElemStr)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append('ð');
            sb.Append(calcElemStr);
            sb.Append('ð');
            return sb.ToString();
        }

        internal static string ConvertToARChargeCode(string calcElemStr)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append('Ť');
            sb.Append(calcElemStr);
            sb.Append('Ť');
            return sb.ToString();
        }

        #region Private classes

        public class TaxCalculationResult
        {
            public decimal TotalAmount { get; internal set; }
            public string[] TaxCodes { get; internal set; }
            public decimal[] AmountOrigin { get; internal set; }
            public decimal[] Percent { get; internal set; }
            public decimal[] CalcTaxAmount { get; internal set; }
        }

        private class TaxGroup
        {
            public TaxGroup(decimal price, decimal assessval, decimal mrp, Dictionary<string, decimal> misccharges)
            {
                Price = price;
                AllTaxBasis = Price;
                IsMRP = false;
                AssessableValue = assessval;
                MRP = mrp;
                if (misccharges != null)
                    MiscCharges = misccharges;
                else
                    MiscCharges = new Dictionary<string, decimal>();
                Taxes = new Dictionary<string, Tax>();
            }
            public decimal Price { get; private set; }
            public decimal AmountOrigin { get; private set; }
            public decimal AllTaxBasis { get; private set; }
            public decimal AssessableValue { get; private set; }
            public decimal MRP { get; private set; }
            public Dictionary<string, decimal> MiscCharges { get; private set; }

            public Dictionary<string, Tax> Taxes { get; private set; }

            public decimal CalculateTaxes()
            {
                decimal items0 = 0;
                decimal items1 = 0;
                foreach (KeyValuePair<string, Tax> t in Taxes)
                {
                    Tax tax = t.Value;
                    tax.CalculateTaxBasisPrepare();
                    if (!tax.PriceInclTax)
                        continue;
                    foreach (decimal val in tax.Items0)
                    {
                        items0 += tax.Tax_Rate * val;
                    }
                    foreach (decimal val in tax.Items1)
                    {
                        items1 += tax.Tax_Rate * val;
                    }
                }
                AmountOrigin = (AllTaxBasis - items0) / (items1 + 1M);
                decimal totaltax = 0;
                foreach (KeyValuePair<string, Tax> t in Taxes)
                {
                    t.Value.CalculateTaxBasis(AmountOrigin);
                    totaltax += t.Value.TaxValue;
                }
                return totaltax;
            }

            internal void TurnOnMRP()
            {
                AllTaxBasis = MRP;
                IsMRP = true;
            }

            public bool IsMRP { get; private set; }

            public void PrintTaxes()
            {
                Console.WriteLine();
                Console.WriteLine("======================================================================");
                Console.WriteLine("Price          : {0}", Price);
                Console.WriteLine("AssessableValue: {0}", AssessableValue);
                Console.WriteLine("MRP            : {0}", MRP);
                foreach (KeyValuePair<string, decimal> misccharge in MiscCharges)
                {
                    Console.WriteLine("Misc. Charge - {0} : {1}", misccharge.Key, misccharge.Value);
                }
                Console.WriteLine("AllTaxBasis    : {0}", AllTaxBasis);
                Console.WriteLine("AmountOrigin   : {0}", AmountOrigin);
                decimal totaltax = 0;
                foreach (KeyValuePair<string, Tax> t in Taxes)
                {
                    Console.WriteLine(t.Value.ToString());
                    totaltax += t.Value.TaxValue;
                }
                Console.WriteLine("======================================================================");
                Console.WriteLine("Total tax: {0}", totaltax);
                Console.WriteLine();
            }

            public bool Proof()
            {
                decimal taxes = AmountOrigin;
                foreach (KeyValuePair<string, Tax> t in Taxes)
                {
                    taxes += t.Value.TaxValue;
                }
                return AllTaxBasis - taxes < 0.009M;
            }
        }

        private class Tax
        {
            public Tax(string code, decimal rate, TaxBasisType basistype, bool priceincltax, string formula, decimal abatementrate, TaxGroup tg)
            {
                Tax_Code = code;
                Tax_Rate = rate;
                Tax_Basis_Type = basistype;
                PriceInclTax = priceincltax;
                taxgroup = tg;
                FormulaItems = new List<Tuple<CalcOperator, CalcElem, string>>();
                CalcFormula = formula;
                CalculationStatus = Status.NotStart;
                Abatement_Rate = abatementrate;
                tg.Taxes[code] = this;
                if (basistype == TaxBasisType.MRP) tg.TurnOnMRP();
            }
            private TaxGroup taxgroup;
            public string Tax_Code { get; private set; }
            public decimal Tax_Rate { get; private set; }
            public decimal Abatement_Rate { get; private set; }
            public TaxBasisType Tax_Basis_Type { get; private set; }
            public Status CalculationStatus { get; private set; }
            private List<decimal> items0 = new List<decimal>();
            public bool PriceInclTax { get; private set; }
            public decimal[] Items0
            {
                get
                {
                    return items0.ToArray();
                }
            }
            private List<decimal> items1 = new List<decimal>();
            public decimal[] Items1
            {
                get
                {
                    return items1.ToArray();
                }
            }
            private string calcFormula;
            public string CalcFormula
            {
                get
                {
                    return calcFormula;
                }
                private set
                {
                    calcFormula = value;
                    char[] formulastr = calcFormula.ToCharArray();
                    int i = 0;
                    while (i < formulastr.Length)
                    {
                        char c = formulastr[i];
                        if (c != '+' && c != '-')
                            throw new Exception("Not valid formula");
                        CalcOperator opt = default(CalcOperator);
                        switch (c)
                        {
                            case '+':
                                opt = CalcOperator.Plus;
                                break;
                            case '-':
                                opt = CalcOperator.Minus;
                                break;
                        }

                        int j = i + 1;
                        if(j < formulastr.Length && formulastr[j] != '¤' && formulastr[j] != 'Ť' && formulastr[j]!='ð')
                            throw new Exception("Not valid formula");

                        StringBuilder sb = new StringBuilder();
                        //for (j = i + 1; j < formulastr.Length && ((formulastr[j] >= 'a' && formulastr[j] <= 'z') || (formulastr[j] >= 'A' && formulastr[j] <= 'Z') || (formulastr[j] >= '0' && formulastr[j] <= '9')); j++)
                        for (j = j + 1; j < formulastr.Length && (formulastr[j] != '¤' && formulastr[j] != 'Ť' && formulastr[j]!='ð'); j++)
                        {
                            sb.Append(formulastr[j]);
                        }

                        if (j >= formulastr.Length || (formulastr[j] != '¤' && formulastr[j] != 'Ť' && formulastr[j] != 'ð'))
                            throw new Exception("Not valid formula");

                        string calcElemStr = sb.ToString();
                        if (formulastr[j] == 'Ť')
                        {
                            calcElemStr = ConvertToARChargeCode(calcElemStr);
                            FormulaItems.Add(new Tuple<CalcOperator, CalcElem, string>(opt, CalcElem.MISC, calcElemStr));
                        }
                        else if (formulastr[j] == 'ð')
                        {
                            calcElemStr = ConvertToAPChargeCode(calcElemStr);
                            FormulaItems.Add(new Tuple<CalcOperator, CalcElem, string>(opt, CalcElem.MISC, calcElemStr));
                        }
                        else if (formulastr[j] == '¤')
                        {
                            FormulaItems.Add(new Tuple<CalcOperator, CalcElem, string>(opt, CalcElem.TaxCode, calcElemStr));
                        }
                        i = j+1;
                    }
                }
            }

            
            public List<Tuple<CalcOperator, CalcElem, string>> FormulaItems { get; private set; }
            public decimal TaxBasisValue { get; set; }
            public decimal TaxValue
            {
                get
                {
                    return Tax_Rate * TaxBasisValue;
                }
            }

            internal void CalculateTaxBasisPrepare()
            {
                if (CalculationStatus == Status.Preparing)
                    throw new Exception("Loop occurs in the formula.");
                if (CalculationStatus == Status.Prepared)
                    return;
                //If the CalculationStatus is NotStart
                CalculationStatus = Status.Preparing;

                switch (Tax_Basis_Type)
                {
                    case TaxBasisType.Line:
                        if (taxgroup.IsMRP)
                            items0.Add(taxgroup.Price * (1 - Abatement_Rate));
                        else
                            items1.Add(1M * (1 - Abatement_Rate));
                        break;
                    case TaxBasisType.ASSESS:
                        items0.Add(taxgroup.AssessableValue * (1 - Abatement_Rate));
                        break;
                    case TaxBasisType.EXLine:
                        break;
                    case TaxBasisType.MRP:
                        items1.Add(1M * (1 - Abatement_Rate));
                        //items0.Add(taxgroup.MRP * (1 - Abatement_Rate));
                        break;
                }

                foreach (Tuple<CalcOperator, CalcElem, string> tuple in FormulaItems)
                {
                    CalcOperator opt = tuple.Item1;
                    CalcElem elem = tuple.Item2;
                    string taxcode = tuple.Item3;

                    int flag = 1;
                    if (opt == CalcOperator.Minus)
                        flag = -1;

                    switch (elem)
                    {
                        case CalcElem.MISC:
                            if (taxgroup.MiscCharges.Keys.Contains(taxcode))
                                items0.Add(flag * taxgroup.MiscCharges[taxcode] * (1 - Abatement_Rate));
                            break;
                        case CalcElem.TaxCode:

                            if(!taxgroup.Taxes.Keys.Contains(taxcode))
                                throw new Exception(string.Format("Tax code {0} does not exist.", taxcode));

                            Tax tax = taxgroup.Taxes[taxcode];
                            tax.CalculateTaxBasisPrepare();
                            foreach (decimal val in tax.Items0)
                            {
                                items0.Add(flag * tax.Tax_Rate * val * (1 - Abatement_Rate));
                            }
                            foreach (decimal val in tax.Items1)
                            {
                                items1.Add(flag * tax.Tax_Rate * val * (1 - Abatement_Rate));
                            }
                            break;
                    }
                }

                CalculationStatus = Status.Prepared;
            }

            internal void CalculateTaxBasis(decimal amountorig)
            {
                if (CalculationStatus == Status.Calculated)
                    return;
                CalculationStatus = Status.Calculating;
                decimal basis = 0;
                if (PriceInclTax)
                {
                    switch (this.Tax_Basis_Type)
                    {
                        case TaxBasisType.Line:
                            if (taxgroup.IsMRP)
                                basis = taxgroup.Price;
                            else
                                basis = amountorig;
                            break;
                        case TaxBasisType.ASSESS:
                            basis = taxgroup.AssessableValue;
                            break;
                        case TaxBasisType.EXLine:
                            break;
                        case TaxBasisType.MRP:
                            basis = amountorig;
                            break;
                    }
                }
                else
                {
                    switch (this.Tax_Basis_Type)
                    {
                        case TaxBasisType.Line:
                            basis = taxgroup.Price;
                            break;
                        case TaxBasisType.ASSESS:
                            basis = taxgroup.AssessableValue;
                            break;
                        case TaxBasisType.MRP:
                            basis = taxgroup.MRP;
                            break;
                    }
                }

                foreach (Tuple<CalcOperator, CalcElem, string> tuple in FormulaItems)
                {
                    CalcOperator opt = tuple.Item1;
                    CalcElem elem = tuple.Item2;
                    string code = tuple.Item3;

                    int flag = 1;
                    if (opt == CalcOperator.Minus)
                        flag = -1;

                    switch (elem)
                    {
                        case CalcElem.MISC:
                            if (taxgroup.MiscCharges.Keys.Contains(code))
                                basis += (flag * taxgroup.MiscCharges[code]);
                            break;
                        case CalcElem.TaxCode:
                            if(!taxgroup.Taxes.Keys.Contains(code))
                                throw new Exception(string.Format("Tax code {0} does not exist.", code));

                            Tax tax = taxgroup.Taxes[code];
                            tax.CalculateTaxBasis(amountorig);
                            basis += (flag * tax.TaxValue);
                            break;
                    }
                }
                this.TaxBasisValue = basis * (1 - Abatement_Rate);
                CalculationStatus = Status.Calculated;
            }

            public override string ToString()
            {
                return string.Format("Tax Code: {0}, Tax Rate: {1}, Tax Basis Type: {2}, Price Incl.: {6}, Calc. Formula: {3}, Tax Basis Value: {4}, Tax Value: {5}", Tax_Code, Tax_Rate, Tax_Basis_Type, CalcFormula, TaxBasisValue, TaxValue, PriceInclTax);
            }
        }

        private enum TaxBasisType
        {
            Line,
            EXLine,
            ASSESS,
            MRP
        }

        private enum CalcOperator
        {
            Plus,
            Minus
            //Multiply,
            //Devide
        }

        private enum CalcElem
        {
            MISC,
            TaxCode
        }

        private enum Status
        {
            NotStart,
            Preparing,
            Prepared,
            Calculating,
            Calculated
        }

        public enum Direction
        {
            None,
            AP,
            AR
        }

        #endregion
    }
}
