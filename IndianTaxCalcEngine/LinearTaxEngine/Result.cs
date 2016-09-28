using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TaxEngine
{
    public class Result
    {
        private readonly IDictionary<string, ResultInfo> variables;

        public decimal? X { get; private set; }

        public Result(IDictionary<string, ResultInfo> results, decimal? x = null)
        {
            if (results == null)
            {
                throw new Exception("Result cannot be NULL.");
            }
            variables = results;
            X = x;
        }

        public ResultInfo this[string i]
        {
            get
            {
                return variables[i];
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(string.Format("X = {0}", X));
            foreach (ResultInfo info in variables.Values)
            {
                sb.AppendLine(info.ToString());
            }
            return sb.ToString();
        }
    }

    public class ResultInfo
    {
        public string Name { get; set; }
        public decimal Amount { get; set; }
        public decimal AmountOrigin { get; set; }
        public override string ToString()
        {
            return string.Format("{0} \t Amount: {1} \t Amount origin: {2}", Name, Amount, AmountOrigin);
        }
    }
}
