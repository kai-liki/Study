using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TaxEngine
{
    internal static class RPNConstantsCalculator
    {
        private static Dictionary<string, Action<Stack<decimal>>> operators = new Dictionary<string, Action<Stack<decimal>>>();
        static RPNConstantsCalculator()
        {
            operators["+"] = new Action<Stack<decimal>>(Plus);
            operators["-"] = new Action<Stack<decimal>>(Minus);
            operators["*"] = new Action<Stack<decimal>>(Multiplies);
        }

        public static decimal Calculate(this string[] tokens, decimal? x, string xname = "X")
        {
            int i = 0;
            Stack<decimal> s = new Stack<decimal>(2);
            while (i < tokens.Length)
            {
                decimal dectoken;
                if (decimal.TryParse(tokens[i], out dectoken))
                {
                    s.Push(dectoken);
                }
                else if (operators.Keys.Contains(tokens[i]))
                {
                    Action<Stack<decimal>> action = operators[tokens[i]];
                    action(s);
                }
                else if (xname.Equals(tokens[i]))
                {
                    if (x == null)
                    {
                        throw new Exception("Failed to calculate X.");
                    }
                    else
                    {
                        s.Push(x.Value);
                    }
                }
                else
                {
                    throw new Exception(string.Format("Unknown type for token {0}", tokens[i]));
                }

#if DEBUG
                Console.WriteLine(string.Format("{0}\t{1}", s.Content(), tokens[i]));
#endif

                i++;
            }

            if (s.Count != 1)
            {
                throw new Exception("Invalid expression.");
            }

            return s.Pop();
        }

        private static void Plus(Stack<decimal> stack)
        {
            if (stack.Count < 2)
            {
                throw new Exception("Invalid expression.");
            }

            decimal v2 = stack.Pop();
            decimal v1 = stack.Pop();
            decimal v = v1 + v2;
            stack.Push(v);
        }

        private static void Minus(Stack<decimal> stack)
        {
            if (stack.Count < 2)
            {
                throw new Exception("Invalid expression.");
            }

            decimal v2 = stack.Pop();
            decimal v1 = stack.Pop();
            decimal v = v1 - v2;
            stack.Push(v);
        }

        private static void Multiplies(Stack<decimal> stack)
        {
            if (stack.Count < 2)
            {
                throw new Exception("Invalid expression.");
            }

            decimal v2 = stack.Pop();
            decimal v1 = stack.Pop();
            decimal v = v1 * v2;
            stack.Push(v);
        }
    }
}
