using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZCL.RTScript.Logic.Expression;

namespace ZCL.RTScript.Logic.Metadata
{
    class RTLibFuncGreater : RTLibMetadata
    {
        public RTLibFuncGreater()
        {
            this.MaxArgNumber = 2;
            this.MinArgNumber = 2; //todo implement as list operation
        }

        protected override object Execute(IList<object> tupple)
        {
            return tupple[0].ToString().CompareTo(tupple[1].ToString()) > 0;
        }

        public override string FunctionName
        {
            get
            {
                return RTSysSymbols.GREATER;
            }
        }
    }
}
