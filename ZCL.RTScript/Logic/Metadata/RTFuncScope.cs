using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZCL.RTScript.Logic.Execution;
using ZCL.RTScript.Logic.Expression;

namespace ZCL.RTScript.Logic.Metadata
{
    public class RTFuncScope : RTMetadata
    {
        public RTFuncScope()
        {
            this.CanDefineVariable = true;
            this.HasReturnValue = false;
        }

        public override string FunctionName
        {
            get { return RTSysSymbols.SCOPE; }
        }

        public override object Execute(Execution.RTExecutionContext context)
        {
            throw new NotImplementedException();
        }
    }
}
