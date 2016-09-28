using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TaxEngine
{
    internal class RPNParser
    {
        private static Dictionary<string, int> Operators = new Dictionary<string, int>(3);

        static RPNParser()
        {
            Operators["+"] = 0;
            Operators["-"] = 0;
            Operators["*"] = 1;
        }

        public void Parse(Context context)
        {
            context.Status = ContextStatus.RPNParsing;
            StringBuilder sb = new StringBuilder();
            foreach (Variable v in context.Variables.Values)
            {
                sb.Clear();
                v.Tokens = new CTree(v.Tokens, 0, v.Tokens.Length - 1).PostorderTrav();
                foreach (string token in v.Tokens)
                {
                    sb.Append(token);
                }
                v.RPN = sb.ToString();
            }

            Equation e = context.CalcEquation;
            if (e != null)
            {
                e.LeftTokens = new CTree(e.LeftTokens, 0, e.LeftTokens.Length - 1).PostorderTrav();
                sb.Clear();
                foreach (string token in e.LeftTokens)
                {
                    sb.Append(token);
                }
                e.LRPN = sb.ToString();

                e.RightTokens = new CTree(e.RightTokens, 0, e.RightTokens.Length - 1).PostorderTrav();
                sb.Clear();
                foreach (string token in e.RightTokens)
                {
                    sb.Append(token);
                }
                e.RRPN = sb.ToString();
            }

            context.Status = ContextStatus.RPNParsed;
        }

        private class Node
        {
            public Node(string token)
            {
                Token = token;
            }
            public string Token { get; private set; }
            public Node Left { get; set; }
            public Node Right { get; set; }
        }

        private class CTree
        {
            public Node Root { get; set; }

            public CTree(string[] tokens, int start, int end)
            {
                if (start > end)
                {
                    throw new Exception("Start index cannot be greater than the end index.");
                }
                int i = start;
                Node preopt = null;
                while (i <= end)
                {
                    string token = tokens[i];
                    Node tempNode;
                    bool subtree = false;
                    if ("(".Equals(token))
                    {
                        int closeindex = i + 1;
                        int bkpair = 1;
                        for (int j = i + 1; j <= end; j++)
                        {
                            if ("(".Equals(tokens[j]))
                            {
                                bkpair++;
                            }
                            else if (")".Equals(tokens[j]))
                            {
                                bkpair--;
                            }
                            if (bkpair == 0)
                            {
                                closeindex = j;
                                break;
                            }
                        }
                        if (bkpair != 0)
                        {
                            throw new Exception("Invalid bracket pair.");
                        }
                        tempNode = new CTree(tokens, i + 1, closeindex - 1).Root;
                        subtree = true;
                        token = tempNode.Token;
                        i = closeindex + 1;
                    }
                    else
                    {
                        tempNode = new Node(token);
                        i++;
                    }

                    if (Operators.Keys.Contains(token) && !subtree)
                    {
                        if (preopt == null)
                        {
                            tempNode.Left = Root;
                            Root = tempNode;
                            preopt = tempNode;
                        }
                        else
                        {
                            Node prenode = Root;
                            if (Operators[prenode.Token] < Operators[token])
                            {
                                tempNode.Left = prenode.Right;
                                prenode.Right = tempNode;
                            }
                            else //if (Operators[prenode.Token] > Operators[token])
                            {
                                tempNode.Left = prenode;
                                Root = tempNode;
                            }
                            /**
                        else
                        {
                            while (prenode.Right != null && prenode.Right.Right != null && Operators[prenode.Right.Token] > Operators[token])
                            {
                                prenode = prenode.Right;
                            }
                            tempNode.Left = prenode.Right;
                            prenode.Right = tempNode;
                        }
                             * **/
                        }
                        preopt = tempNode;
                    }
                    else
                    {
                        if (preopt == null)
                        {
                            Root = tempNode;
                        }
                        else if (preopt.Left == null)
                        {
                            preopt.Left = tempNode;
                        }
                        else
                        {
                            preopt.Right = tempNode;
                        }
                    }
                }
            }

            private string[] PostorderTrav(Node root)
            {
                List<string> target = new List<string>();
                if (root.Left != null)
                {
                    target.AddRange(PostorderTrav(root.Left));
                }
                if (root.Right != null)
                {
                    target.AddRange(PostorderTrav(root.Right));
                }
                target.Add(root.Token);

                return target.ToArray();
            }

            public string[] PostorderTrav()
            {
                if (Root == null)
                {
                    throw new Exception("Root is NULL");
                }
                return PostorderTrav(Root);
            }
        }
    }
}
