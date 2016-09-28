using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TaxEngine
{
    internal static class Util
    {
        public static string Content(this Stack<decimal> s)
        {
            IEnumerator<decimal> e = s.GetEnumerator();
            StringBuilder sb = new StringBuilder();
            while (e.MoveNext())
            {
                sb.Append(e.Current.ToString());
                sb.Append('|');
            }
            return sb.ToString();
        }

        public static string Content(this Stack<decimal?> s)
        {
            IEnumerator<decimal?> e = s.GetEnumerator();
            StringBuilder sb = new StringBuilder();
            while (e.MoveNext())
            {
                if (e.Current == null)
                {
                    sb.Append("NULL");
                }
                else
                {
                    sb.Append(e.Current.Value.ToString());
                }
                sb.Append('|');
            }
            return sb.ToString();
        }

        public static void PrintANXResult(this ANXResult result)
        {
            Console.WriteLine(string.Format("Item1 : {0} \t Item0 : {1}", result.Item1, result.Item0));
        }
    }
}
