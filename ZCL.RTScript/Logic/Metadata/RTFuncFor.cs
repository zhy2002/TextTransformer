
using ZCL.RTScript.Logic.Execution;
using ZCL.RTScript.Logic.Expression;

namespace ZCL.RTScript.Logic.Metadata
{
    public class RTFuncFor : RTMetadata
    {
        public RTFuncFor()
        {
 
            this.CanDefineVariable = true;
            this.HasReturnValue = true;
 
        }

        public override string FunctionName
        {
            get { return RTSysSymbols.FOR; }
        }

        public override object Execute(Execution.RTExecutionContext context)
        {
            RTScope func = context.CurrentScope;
            int step = 1;
            string retval = string.Empty;
            var varname = func.GetArg(0) as RTVariable;
            var from = (int)RTConverter.Singleton.ToNumber(EvaluateArg(1, context));
            var to = (int)RTConverter.Singleton.ToNumber(EvaluateArg(2, context));
            if (from > to) step = -1;

            var body = func.GetArg(3) as RTExpression;

            for (int i = from; step == -1 ? i >= to : i <= to; i += step)
            {
                context.CurrentScope.SetValueLocal(varname.Name, (double)i);
                var result = body.Execute(context);
                if (!(result is RTVoid))
                {
                    retval += RTConverter.Singleton.ToString(result);
                }
            }

            return retval;

        }
    }
}
