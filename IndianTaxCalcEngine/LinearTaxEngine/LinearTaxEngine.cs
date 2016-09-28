using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace TaxEngine
{
    public class LinearTaxEngine
    {
        public static Result Calculate(string text)
        {
            Context context;

            ExpParser expparser = new ExpParser();
            RPNParser rpnparser = new RPNParser();
            Exp1Parser exp1parser = new Exp1Parser();

            context = expparser.Parse(text);
            rpnparser.Parse(context);
            exp1parser.Parse(context);

            Result result = Calculate(context);

            return result;
        }

        private static Result Calculate(Context context)
        {
            decimal? x;
            Equation e = context.CalcEquation;
            if (e != null)
            {
                ANXResult lresult = e.LeftTokens.Calculate(context.XName);
                ANXResult rresult = e.RightTokens.Calculate(context.XName);
                e.Item0 = lresult.Item0 - rresult.Item0;
                e.Item1 = lresult.Item1 - rresult.Item1;
                x = e.GetX();
            }
            else
            {
                x = null;
            }
            Dictionary<string, ResultInfo> results = new Dictionary<string, ResultInfo>();
            foreach (KeyValuePair<string, Variable> vp in context.Variables)
            {
                string vname = vp.Key;
                Variable v = vp.Value;
                ResultInfo resultinfo = new ResultInfo();
                resultinfo.Name = vname;
                resultinfo.Amount = v.GetVariableValue(x, context.XName);
                results[vname] = resultinfo;
            }

            Result result = new Result(results, x);
            return result;
        }
    }
}
