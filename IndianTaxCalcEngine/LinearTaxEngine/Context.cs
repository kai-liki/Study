using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TaxEngine
{
    internal class Context
    {
        private readonly string text;
        private string xname = "X";

        public Context(string txt)
        {
            text = txt;
            Variables = new Dictionary<string, Variable>();
            Status = ContextStatus.Uninitialized;
        }

        public string XName
        {
            get
            {
                return xname;
            }
        }

        public void UpdateXName(ExpParser parser)
        {
            xname = parser.GetXName();
        }

        public string Text
        {
            get
            {
                return text;
            }
        }

        public IDictionary<string, Variable> Variables
        {
            get;
            private set;
        }

        public Equation CalcEquation
        {
            get;
            set;
        }

        public ContextStatus Status
        {
            get;
            set;
        }
    }
}
