using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZCL.RTScript.Logic.Execution;
using ZCL.RTScript.Logic.Expression;

namespace ZCL.RTScript.Logic.Metadata
{
    public class RTFuncSet : RTMetadata
    {
        public RTFuncSet()
        {
 
            this.CanDefineVariable = false;
            this.HasReturnValue = false;
 
        }

        public override string FunctionName
        {
            get { return RTSysSymbols.SET; }
        }

        public override object Execute(Execution.RTExecutionContext context)
        {
            RTScope func = context.CurrentScope;
            bool hasError = false;
            var caller = context.CurrentScope.Parent.Creator as RTFunction;
            if (caller != null && !caller.Metadata.CanDefineVariable)
            {
                context.Errors.Add(new Execution.RTExecutionError(func, RTErrorCode.SysFuncError, ErrorMessages.Function_Def_Caller_No_Scope));
                hasError = true;
            }

            var symbol = func.GetArg(0) as RTVariable;
            if (symbol == null)
            {
                context.Errors.Add(new Execution.RTExecutionError(func, RTErrorCode.SysFuncError, ErrorMessages.Function_Def_Arg0));
                hasError = true;
            }

            if (!hasError)
            {
                object value = this.EvaluateArg(1, context);
                context.CurrentScope.Parent.SetValue(symbol.Name, value);
            }
            return RTVoid.Singleton;
        }
    }
}
