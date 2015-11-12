
using ZCL.RTScript.Logic.Expression;

namespace ZCL.RTScript.Logic.Execution
{
    public class RTExecutionError : RTParsingError
    {
        internal RTExecutionError(RTScope func, RTErrorCode errCode, string errMsg)
            : base(func.Creator.SourceStartIndex, func.Creator.SourceEndIndex, errCode, errMsg)
        {
            this.FunctionName = func.FunctionName;
        }

        public string FunctionName
        {
            get;
            private set;
        }
    }
}
