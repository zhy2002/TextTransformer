using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZCL.RTScript.Logic.Execution;
using ZCL.RTScript.Logic.Expression;

namespace ZCL.RTScript.Logic.Metadata
{
    public class RTFuncApply : RTMetadata
    {
        public override string FunctionName
        {
            get { return RTSysSymbols.APPLY; }
        }

        public RTFuncApply()
        {
            this.CanDefineVariable = true;
            this.HasReturnValue = true;
        }

        public override void NormalizeArgs(IList<object> args)
        {
            args[0] = (args[0] as RTVariable).Name;
        }


        public override object Execute(Execution.RTExecutionContext context)
        {
            RTScope scope = context.CurrentScope;
            int i;

            string varname = this.EvaluateArg(0, context).ToString();
            object target = new RTVariable(varname).Execute(context);//resolve the name

            string targetName = target as string;
            if (targetName != null)
            {
                RTMetadata metadata = context.MetadataFactory.GetMetadata(targetName);
                if (metadata != null)
                {
                    scope.RemoveArg(0);
                    return metadata.Execute(context);
                }
                else
                {
                    context.Errors.Add(new Execution.RTExecutionError(scope, RTErrorCode.SysFuncError, string.Format(ErrorMessages.Function_Apply_No_Metadata, targetName)));
                    return RTVoid.Singleton;
                }
            }

            RTScope targetScope = target as RTScope;
            if (targetScope != null)
            {
                List<RTVariable> vars = new List<RTVariable>();
                targetScope.GetAllParameters(vars);
                vars.Sort(_comparer);

                for (i = 0; i < vars.Count; i++)
                {
                    if (i + 1 == scope.ArgCount) break;
                    scope.SetValueLocal(vars[i].Name, EvaluateArg(i + 1, context));
                }
                return targetScope.Execute(context);
            }

            RTVariable targetVar = target as RTVariable; //recursion
            if (context.DefiningSymbol != null && targetVar.Name == context.DefiningSymbol.Name)
            {
                scope.AddArg(varname);
                for (i = 1; i < scope.Creator.Args.Count; i++)
                {
                    scope.AddArg(EvaluateArg(i, context));
                }

                return scope;
            }
            else
            {
                context.Errors.Add(new Execution.RTExecutionError(scope, RTErrorCode.SysFuncError, ErrorMessages.Function_Apply_Invalid_Target));
                return RTVoid.Singleton;
            }


        }


        private static IComparer<RTVariable> _comparer = new ArgNameComparer();

        private class ArgNameComparer : IComparer<RTVariable>
        {
            public int Compare(RTVariable x, RTVariable y)
            {
                string keyX = string.Empty;
                string keyY = keyX;
                int index = x.Name.LastIndexOf("$");
                if (index != -1)
                {
                    keyX += x.Name.Substring(index);
                }

                index = y.Name.LastIndexOf("$");
                if (index != -1)
                {
                    keyX += x.Name.Substring(index);
                }

                if (keyX == keyY) return 0;
                if (keyX == string.Empty) return 1;
                if (keyY == string.Empty) return -1;
                return keyX.CompareTo(keyY);
            }
        }

    }
}
