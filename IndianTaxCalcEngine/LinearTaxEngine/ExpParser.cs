using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace TaxEngine
{
    internal class ExpParser
    {
        private static char[] operators = new char[] { '+', '-', '*', '(', ')', '=' };
        private const char DEFINESYMBOL = '#';
        private const char EQUATIONOPEN = '[';
        private const char EQUATIONCLOSE = ']';

        private const string REGEX_DEFINEEXPRESSION = @"^#[a-zA-Z_]+[a-zA-Z0-9_]*$";
        private const string REGEX_VARNAME = @"^[a-zA-Z_]+[a-zA-Z0-9_]*$";
        private const string REGEX_NUMBER = @"^0$|^[-]?0\.[0-9]+$|^[-]?[1-9]+[0-9]*\.[0-9]+$|^[-]?[1-9]+[0-9]*$";
        private const string REGEX_EXPRESSION = @"^[a-zA-Z0-9_ ()+\-*.]*$";
        private const string REGEX_FULLEQUATION = @"^\[[a-zA-Z0-9_ ()+\-*.]+=[a-zA-Z0-9_ ()+\-*.]*\]$";
        private const string REGEX_EQUATION = @"^[a-zA-Z0-9_ ()+\-*.]+=[a-zA-Z0-9_ ()+\-*.]+$";
        private const string REGEX_ASSIGNMENT = @"^[a-zA-Z_]+[a-zA-Z0-9_]*[ ]*=[a-zA-Z0-9_ ()+\-*.]*$";

        /**
        private const string REGEX_DEFINEEXPRESSION = @"^#[a-zA-Z\_]+[a-zA-Z0-9\_]*$";
        private const string REGEX_VARNAME = @"^[a-zA-Z\_]+[a-zA-Z0-9\_]*$";
        private const string REGEX_NUMBER = @"^0$|^0\.[0-9]+$|^[1-9]+[0-9]*\.[0-9]+$|^[1-9]+[0-9]*$";
        private const string REGEX_EXPRESSION = @"^[a-zA-Z0-9\_ ()+\-*.]*$";
        private const string REGEX_FULLEQUATION = @"^\[[a-zA-Z0-9\_ ()+\-*.]+=[a-zA-Z0-9\_ ()+\-*.]*\]$";
        private const string REGEX_EQUATION = @"^[a-zA-Z0-9\_ ()+\-*.]+=[a-zA-Z0-9\_ ()+\-*.]*$";
        private const string REGEX_ASSIGNMENT = @"^[a-zA-Z\_]+[a-zA-Z0-9\_]*[ ]*=[a-zA-Z0-9\_ ()+\-*.]*$";
        **/

        private string xname = "X";

        public Context Parse(string text)
        {
            StringBuilder sb = new StringBuilder();
            char[] buffer = new char[1];

            Context context = new Context(text);

            context.Status = ContextStatus.ExpParsing;

            using (StringReader sr = new StringReader(text))
            {
                while (sr.Peek() != -1)
                {
                    string line = sr.ReadLine().Trim();

                    if (line.Length <= 0)
                    {
                        continue;
                    }
                    else if (line.StartsWith(DEFINESYMBOL.ToString()))
                    {
                        AssertMatch(line, REGEX_DEFINEEXPRESSION, "Invalid X name defination");
                        if (line.Length <= 1)
                        {
                            throw new Exception("Invalid X name defination");
                        }
                        line = line.Substring(1);
                        AssertMatch(line, REGEX_VARNAME, "Invalid X name");
                        if (context.Variables.Count > 0 || context.CalcEquation != null)
                        {
                            throw new Exception("X name defination must be put in front of any variable defination or equation defination.");
                        }
                        xname = line;
                        context.UpdateXName(this);
                    }
                    else if (line.StartsWith(EQUATIONOPEN.ToString()))
                    {
                        AssertMatch(line, REGEX_FULLEQUATION, "Invalid equation defination");

                        line = line.Substring(1, line.Length - 2).Trim();

                        AssertMatch(line, REGEX_EQUATION, "Invalid equation");

                        Equation equation = new Equation();
                        equation.Exp = line;

                        string[] exps = line.Split('=');
                        string leftexp = exps[0].Trim();
                        string rightexp = exps[1].Trim();
                        equation.LeftTokens = Tokenize(leftexp);
                        equation.RightTokens = Tokenize(rightexp);

                        context.CalcEquation = equation;
                    }
                    else
                    {
                        AssertMatch(line, REGEX_ASSIGNMENT, "Invalid variable assignment");
                        
                        string[] exps = line.Split('=');
                        string varname = exps[0].Trim();
                        string expression = exps[1].Trim();

                        AssertMatch(expression, REGEX_EXPRESSION, "Invalid variable expression");

                        if (context.Variables.Keys.Contains(varname))
                        {
                            throw new Exception(string.Format("Variable {0} has already been defined.", varname));
                        }

                        Variable variable =new Variable();
                        variable.Exp = expression;
                        variable.Tokens = Tokenize(expression);
                        context.Variables[varname] = variable;
                        
                    }
                }
            }

            context.Status = ContextStatus.ExpParsed;
            return context;
        }

        public string GetXName()
        {
            return xname;
        }

        private void AssertMatch(string str, string pattern, string message="Invalid expression.")
        {
            if (!Regex.IsMatch(str, pattern))
            {
                throw new Exception(string.Format("{2} - String {0} does not match pattern {1}.", str, pattern, message));
            }
        }

        private string[] Tokenize(string text)
        {
            List<string> tokens = new List<string>();
            char[] buffer = new char[1];
            using (StringReader linesr = new StringReader(text))
            {
                StringBuilder sb = new StringBuilder();
                int bracketlevel = 0;
                string str;
                while (linesr.Peek() != -1)
                {
                    linesr.Read(buffer, 0, 1);
                    char c = buffer[0];
                    
                    if ((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || (c >= '0' && c <= '9') || c == '.' || c == ' ')
                    {
                        sb.Append(c);
                    }
                    else
                    {
                        str = str = sb.ToString().Trim();
                        switch (c)
                        {
                            case '-':
                                if (!string.IsNullOrWhiteSpace(str))
                                {
                                    if (!Regex.IsMatch(str, REGEX_VARNAME) && !Regex.IsMatch(str, REGEX_NUMBER))
                                    {
                                        throw new Exception(string.Format("Invalid operant {0}. It is neither a valid variable name nor a valid number. {1}", str, text));
                                    }
                                    tokens.Add(str);
                                    tokens.Add(c.ToString());
                                    sb.Clear();
                                }
                                else if (tokens.Count > 0 &&
                                            (")".Equals(tokens[tokens.Count - 1]) ||
                                                Regex.IsMatch(tokens[tokens.Count - 1], REGEX_VARNAME) &&
                                                Regex.IsMatch(tokens[tokens.Count - 1], REGEX_NUMBER)
                                            )
                                        )
                                {

                                    tokens.Add(c.ToString());
                                }
                                else if ((tokens.Count > 0 && "(".Equals(tokens[tokens.Count - 1])) || tokens.Count == 0)
                                {
                                    sb.Append(c);
                                }
                                else
                                {
                                    throw new Exception(string.Format("Invalid operator {0}. It is not following a valid variable or number. {1}", c, text));
                                }
                                break;
                            case '+':
                            case '*':
                                if (!string.IsNullOrWhiteSpace(str))
                                {
                                    if (!Regex.IsMatch(str, REGEX_VARNAME) && !Regex.IsMatch(str, REGEX_NUMBER))
                                    {
                                        throw new Exception(string.Format("Invalid operant {0}. It is neither a valid variable name nor a valid number. {1}", str, text));
                                    }
                                    tokens.Add(str);
                                }
                                else if (tokens.Count <= 0 ||
                                            (!")".Equals(tokens[tokens.Count - 1]) &&
                                                !Regex.IsMatch(tokens[tokens.Count - 1], REGEX_VARNAME) &&
                                                !Regex.IsMatch(tokens[tokens.Count - 1], REGEX_NUMBER)
                                            )
                                        )
                                {
                                    throw new Exception(string.Format("Invalid operator {0}. It is not following a valid variable or number. {1}", c, text));
                                }

                                tokens.Add(c.ToString());
                                sb.Clear();
                                break;
                            case '(':
                                if (!string.IsNullOrWhiteSpace(str))
                                {
                                    throw new Exception(string.Format("Invalid open bracket. {0}", text));
                                }
                                bracketlevel++;
                                tokens.Add(c.ToString());
                                sb.Clear();
                                break;
                            case ')':
                                if (!string.IsNullOrWhiteSpace(str))
                                {
                                    if (!Regex.IsMatch(str, REGEX_VARNAME) && !Regex.IsMatch(str, REGEX_NUMBER))
                                    {
                                        throw new Exception(string.Format("Invalid operant {0}. It is neither a valid variable name nor a valid number. {1}", str, text));
                                    }
                                    tokens.Add(str);
                                    tokens.Add(c.ToString());
                                }
                                else if (tokens.Count > 0 &&
                                            (")".Equals(tokens[tokens.Count - 1]) ||
                                                Regex.IsMatch(tokens[tokens.Count - 1], REGEX_VARNAME) &&
                                                Regex.IsMatch(tokens[tokens.Count - 1], REGEX_NUMBER)
                                            )
                                        )
                                {

                                    tokens.Add(c.ToString());
                                }
                                else
                                {
                                    throw new Exception(string.Format("Invalid closing bracket. {0}", text));
                                }

                                bracketlevel--;
                                if (bracketlevel < 0)
                                {
                                    throw new Exception(string.Format("Invalid bracket pair. {0}", text));
                                }

                                sb.Clear();
                                break;
                            default:
                                throw new Exception(string.Format("Invalid operator in expression. {0}", text));
                        }
                    }
                }

                if (bracketlevel != 0)
                {
                    throw new Exception(string.Format("Invalid bracket pair. {0}", text));
                }

                string tempstr = sb.ToString().Trim();
                if (!string.IsNullOrWhiteSpace(tempstr))
                {
                    if (!Regex.IsMatch(tempstr, REGEX_VARNAME) && !Regex.IsMatch(tempstr, REGEX_NUMBER))
                    {
                        throw new Exception(string.Format("Invalid operant {0}. It is neither a valid variable name nor a valid number. {1}", tempstr, text));
                    }
                    tokens.Add(tempstr);
                }
                sb.Clear();
            }

            return tokens.ToArray();
        }
    }
}
