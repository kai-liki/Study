using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IndianTaxCalcEngine
{
    public interface ITaxBasisCalculation
    {
        void CalculateTaxBasisPrepare();
        decimal CalculateTaxBasis(decimal amountOrigin);
        decimal CalculateTaxAmount();
        decimal GetLinearTerm();
        decimal GetConstantTerm();
    }
}
