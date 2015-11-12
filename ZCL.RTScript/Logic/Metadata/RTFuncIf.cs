using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZCL.RTScript.Logic.Execution;
using ZCL.RTScript.Logic.Expression;

namespace ZCL.RTScript.Logic.Metadata
{
    public class RTFuncIf : RTMetadata
    {
        public RTFuncIf()
        {

            this.CanDefineVariable = true;
            this.HasReturnValue = true;

        }

        public override string FunctionName
        {
            get { return RTSysSymbols.IF; }
        }

        public override object Execute(Execution.RTExecutionContext context)
        {
            RTScope scope = context.CurrentScope;
            //check
            if (scope.ArgCount < 2)
            {
                context.Errors.Add(new RTExecutionError(scope, RTErrorCode.Metadata_Not_Enough_Args, string.Format(ErrorMessages.Metadata_Not_Enough_Args, 2, scope.ArgCount)));
            }

            if (scope.ArgCount > 3)
            {
                context.Errors.Add(new RTExecutionError(scope, RTErrorCode.Metadata_Too_Many_Args, string.Format(ErrorMessages.Metadata_Too_Many_Args, 3, scope.ArgCount)));
            }

            var condition = EvaluateArg(0, context);
            var conditionScope = condition as RTScope;
            if (conditionScope == null)
            {
                bool? cond = RTConverter.Singleton.ToBoolean(condition);
                if (cond.HasValue)
                {
                    if (cond.Value)
                    {
                        return EvaluateArg(1, context);
                    }
                    else
                    {
                        if (scope.ArgCount < 3) return RTVoid.Singleton;
                        else return EvaluateArg(2, context);
                    }
                }
                else
                {
                    context.Errors.Add(new RTExecutionError(scope, RTErrorCode.SysFuncError, ErrorMessages.Function_If_Null_Condition));
                    return RTVoid.Singleton;
                }
            }
            else
            {
                scope.AddArg(condition);
                scope.AddArg(EvaluateArg(1, context));
                if (scope.Creator.Args.Count > 2)
                    scope.AddArg(EvaluateArg(2, context));
                return scope;

            }

        }
    }
}
