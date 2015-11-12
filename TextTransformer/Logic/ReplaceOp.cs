using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RexReplace.GUI.Logic
{
    internal class ReplaceOp
    {
        public int MinArgNumber
        {
            get;
            private set;
        }

        public int MaxArgNumber
        {
            get;
            private set;
        }

        public Func<List<object>, IReplaceContext, object> Body
        {
            get;
            private set;
        }

        internal ReplaceOp(Func<List<object>, IReplaceContext, object> body, int minArgNumber = 1, int maxArgNumber = 1)
        {
            MinArgNumber = minArgNumber;
            MaxArgNumber = maxArgNumber;
            Body = body;
        }
    }
}
