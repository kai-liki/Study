using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TaxEngine
{
    internal static class RPNANXCalculator
    {
        private static Dictionary<string, Action<Stack<decimal?>, Stack<decimal>, Stack<decimal>>> operators = new Dictionary<string, Action<Stack<decimal?>, Stack<decimal>, Stack<decimal>>>();
        static RPNANXCalculator()
        {
            operators["+"] = new Action<Stack<decimal?>, Stack<decimal>, Stack<decimal>>(Plus);
            operators["-"] = new Action<Stack<decimal?>, Stack<decimal>, Stack<decimal>>(Minus);
            operators["*"] = new Action<Stack<decimal?>, Stack<decimal>, Stack<decimal>>(Multiplies);
        }

        public static ANXResult Calculate(this string[] tokens, string xname = "X")
        {
            int i = 0;
            Stack<decimal?> s = new Stack<decimal?>(2);
            Stack<decimal> i0 = new Stack<decimal>();
            Stack<decimal> i1 = new Stack<decimal>();
            
            while (i < tokens.Length)
            {
                decimal dectoken;
                if (decimal.TryParse(tokens[i], out dectoken))
                {
                    s.Push(dectoken);
                }
                else if (operators.Keys.Contains(tokens[i]))
                {
                    Action<Stack<decimal?>, Stack<decimal>, Stack<decimal>> action = operators[tokens[i]];
                    action(s, i1, i0);
                }
                else if (xname.Equals(tokens[i]))
                {
                    s.Push(null);
                    i1.Push(1);
                    i0.Push(0);
                }
                else
                {
                    throw new Exception(string.Format("Unknown type for token {0}", tokens[i]));
                }
#if DEBUG
                Console.WriteLine(string.Format("{4}\t{0}\t{1}\t{2}\t{3}", s.Content(), tokens[i], i1.Content(), i0.Content(), i));
#endif
                i++;
            }

            if (s.Count != 1)
            {
                throw new Exception("Invalid expression.");
            }

            decimal sc = s.Sum((d2) => { if (d2 != null) return d2.Value; else return 0; });

            ANXResult result = new ANXResult() { Item1 = i1.Sum(), Item0 = i0.Sum() + sc };

            return result;
        }

        private static void Plus(Stack<decimal?> stack, Stack<decimal> item1, Stack<decimal> item0)
        {
            if (stack.Count < 2)
            {
                throw new Exception("Invalid expression.");
            }

            decimal? v2 = stack.Pop();
            decimal? v1 = stack.Pop();
            if (v1 != null && v2 != null)
            {
                decimal v = v1.Value + v2.Value;
                stack.Push(v);
            }
            else if (v1 == null && v2 == null)
            {
                if (item1.Count < 2)
                {
                    throw new Exception("Invalid expression.");
                }

                decimal i11 = item1.Pop();
                decimal i12 = item1.Pop();
                item1.Push(i11 + i12);

                decimal i01 = item0.Pop();
                decimal i02 = item0.Pop();
                item0.Push(i01 + i02);

                stack.Push(null);
            }
            else
            {
                if (item0.Count > 0)
                {
                    item0.Push(item0.Pop() + (v1 == null ? v2.Value : v1.Value));
                }
                else
                {
                    item0.Push(v1 == null ? v2.Value : v1.Value);
                }

                stack.Push(null);
            }
        }

        private static void Minus(Stack<decimal?> stack, Stack<decimal> item1, Stack<decimal> item0)
        {
            if (stack.Count < 2)
            {
                throw new Exception("Invalid expression.");
            }

            decimal? v2 = stack.Pop();
            decimal? v1 = stack.Pop();
            if (v1 != null && v2 != null)
            {
                decimal v = v1.Value - v2.Value;
                stack.Push(v);
            }
            else if (v1 == null && v2 == null)
            {
                if (item1.Count < 2)
                {
                    throw new Exception("Invalid expression.");
                }

                decimal i12 = item1.Pop();
                decimal i11 = item1.Pop();
                item1.Push(i11 - i12);

                decimal i02 = item0.Pop();
                decimal i01 = item0.Pop();
                item0.Push(i01 - i02);

                stack.Push(null);
            }
            else if (v1 == null)
            {
                if (item0.Count > 0)
                {
                    item0.Push(item0.Pop() - v2.Value);
                }
                else
                {
                    item0.Push(-1 * v2.Value);
                }

                stack.Push(null);
            }
            else
            {
                if (item0.Count > 0)
                {
                    item0.Push(item0.Pop() + v1.Value);
                }
                else
                {
                    item0.Push(v1.Value);
                }
                item1.Push(-1 * item1.Pop());

                stack.Push(null);
            }
        }

        private static void Multiplies(Stack<decimal?> stack, Stack<decimal> item1, Stack<decimal> item0)
        {
            if (stack.Count < 2)
            {
                throw new Exception("Invalid expression.");
            }

            decimal? v2 = stack.Pop();
            decimal? v1 = stack.Pop();
            if (v1 == null && v2 == null)
            {
                throw new Exception("Invalid expression. Not linear.");
            }
            else if (v1 != null && v2 != null)
            {
                decimal v = v1.Value * v2.Value;
                stack.Push(v);
            }
            else
            {
                if (item1.Count > 0)
                {
                    decimal v = item1.Pop();
                    item1.Push(v * (v1 == null ? v2.Value : v1.Value));
                }

                if (item0.Count > 0)
                {
                    decimal v = item0.Pop();
                    item0.Push(v * (v1 == null ? v2.Value : v1.Value));
                }

                stack.Push(null);
            }
        }
    }
}
