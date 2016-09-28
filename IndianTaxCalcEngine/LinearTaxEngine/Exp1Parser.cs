using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace TaxEngine
{
    internal class Exp1Parser
    {
        private const string REGEX_NUMBER = @"^0$|^[-]?0\.[0-9]+$|^[-]?[1-9]+[0-9]*\.[0-9]+$|^[-]?[1-9]+[0-9]*$";
        private const string REGEX_OPERATOR = @"^[+\-*]$";

        private Dictionary<Variable, VariableStatus> varstatus;
        public void Parse(Context context)
        {
            context.Status = ContextStatus.Exp1Parsing;

            varstatus = new Dictionary<Variable, VariableStatus>();

            StringBuilder sb = new StringBuilder();
            foreach (string varname in context.Variables.Keys)
            {
                sb.Clear();
                context.Variables[varname].Tokens = GetAnXExp(context, varname);
                foreach (string token in context.Variables[varname].Tokens)
                {
                    sb.Append(token);
                }
                context.Variables[varname].Exp1 = sb.ToString();
            }

            Equation equation = context.CalcEquation;
            if (equation != null)
            {
                equation.LeftTokens = GetAnXExp(context, equation.LeftTokens);
                sb.Clear();
                foreach (string token in equation.LeftTokens)
                {
                    sb.Append(token);
                }
                equation.LExp = sb.ToString();

                equation.RightTokens = GetAnXExp(context, equation.RightTokens);
                sb.Clear();
                foreach (string token in equation.RightTokens)
                {
                    sb.Append(token);
                }
                equation.RExp = sb.ToString();
            }
            context.Status = ContextStatus.Exp1Parsed;
        }

        private string[] GetAnXExp(Context context, string[] tokens)
        {
            List<string> target = new List<string>();

            foreach (string token in tokens)
            {
                if (context.XName.Equals(token))
                {
                    target.Add(token);
                }
                else if (context.Variables.Keys.Contains(token))
                {
                    target.AddRange(GetAnXExp(context, token));
                }
                else
                {
                    if (Regex.IsMatch(token, REGEX_NUMBER) || Regex.IsMatch(token, REGEX_OPERATOR))
                    {
                        target.Add(token);
                    }
                    else
                    {
                        throw new Exception(string.Format("Variable {0} has never been defined.", token));
                    }
                }
            }

            return target.ToArray();
        }

        private string[] GetAnXExp(Context context, string varname)
        {
            if(!context.Variables.Keys.Contains(varname) && !context.XName.Equals(varname))
            {
                throw new Exception(string.Format("Variable {0} has not been defined.", varname));
            }

            Variable variable = context.Variables[varname];
            if (varstatus.Keys.Contains(variable) && varstatus[variable] == VariableStatus.Exp1Parsed)
            {
                return variable.Tokens;
            }
            else if (varstatus.Keys.Contains(variable) && varstatus[variable] == VariableStatus.Exp1Parsing)
            {
                throw new Exception(string.Format("Loop occur for variable {0}", varname));
            }
            else
            {
                varstatus[variable] = VariableStatus.Exp1Parsing;
            }

            List<string> target = new List<string>();

            string[] tokens = variable.Tokens;

            foreach (string token in tokens)
            {
                if (context.XName.Equals(token))
                {
                    target.Add(token);
                }
                else if (context.Variables.Keys.Contains(token))
                {
                    target.AddRange(GetAnXExp(context, token));
                }
                else
                {
                    if (Regex.IsMatch(token, REGEX_NUMBER) || Regex.IsMatch(token, REGEX_OPERATOR))
                    {
                        target.Add(token);
                    }
                    else
                    {
                        throw new Exception(string.Format("Variable {0} has never been defined.", token));
                    }
                }
            }

            variable.Tokens = target.ToArray();
            varstatus[variable] = VariableStatus.Exp1Parsed;
            return variable.Tokens;
        }
    }
}
