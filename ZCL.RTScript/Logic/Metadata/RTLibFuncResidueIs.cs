using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZCL.RTScript.Logic.Execution;
using ZCL.RTScript.Logic.Expression;

namespace ZCL.RTScript.Logic.Metadata
{
    public class RTLibFuncResidueIs : RTLibMetadata
    {
        public RTLibFuncResidueIs()
        {
            this.MaxArgNumber = 3;
            this.MinArgNumber = 3;
        }

        public override string FunctionName
        {
            get { return RTSysSymbols.RESIDUE_IS; }
        }

        protected override object Execute(IList<object> args)
        {
            double? arg1 = RTConverter.Singleton.ToNumber(args[0]);
            if (!arg1.HasValue) return double.NaN;
            double? arg2 = RTConverter.Singleton.ToNumber(args[1]);
            if (!arg2.HasValue) return double.NaN;
            double? arg3 = RTConverter.Singleton.ToNumber(args[2]);
            if (!arg3.HasValue) return double.NaN;

            return (int)arg1.Value % (int)arg2.Value == (int)arg3.Value;
        }
    }
}
