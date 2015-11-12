using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZCL.RTScript.Logic.Expression;

namespace ZCL.RTScript.Logic.Metadata
{
    class RTLibFuncNotEqual : RTLibMetadata
    {
        public RTLibFuncNotEqual()
        {
            this.MaxArgNumber = 2;
            this.MinArgNumber = 2;
        }

        protected override object Execute(IList<object> tupple)
        {
            return tupple[0].ToString() != tupple[1].ToString();
        }

        public override string FunctionName
        {
            get
            {
                return RTSysSymbols.NOT_EQUAL;
            }
        }
    }
}
