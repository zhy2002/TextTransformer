using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZCL.RTScript.AbstractionLayer;
using ZCL.RTScript.Logic.Execution;
using ZCL.RTScript.Logic.Expression;

namespace ZCL.RTScript.Logic.Metadata
{
    class RTFuncGet : RTMetadata
    {
        public RTFuncGet()
        {
 
            this.CanDefineVariable = false;
            this.HasReturnValue = true;
 
        }

        public override string FunctionName
        {
            get
            {
                return RTSysSymbols.GET;
            }
        }

        public override object Execute(Execution.RTExecutionContext context)
        {
            RTScope func = context.CurrentScope;
            var index = (int)RTConverter.Singleton.ToNumber(EvaluateArg(0, context));
            if (index < 0) return null;

            RTScope root = context.RootScope;
            IRTMatch match = root.GetValueLocal(RTSysSymbols.MATCH_INPUT) as IRTMatch;
            if (index >= match.Count) return null;
            return match.GetItem(index);
        }
    }
}
