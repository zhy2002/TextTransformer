using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZCL.RTScript.Logic.Execution;
using ZCL.RTScript.Logic.Expression;

namespace ZCL.RTScript.Logic.Metadata
{
    public class RTLibFuncMul : RTLibMetadata
    {
        public override string FunctionName
        {
            get { return RTSysSymbols.MUL; }
        }


        protected override object Execute(IList<object> args)
        {
            double product = 1;
            for (int i = 0; i < args.Count; i++)
            {
                double? val = RTConverter.Singleton.ToNumber(args[i]);
                if (val.HasValue)
                {
                    product *= val.Value;
                }
                else
                {
                    return double.NaN;
                }
            }
            return product;
        }
    }
}
