
using ZCL.RTScript.Logic.Execution;

namespace ZCL.RTScript.AbstractionLayer
{
    public interface IRTExpression
    {
        object Execute(RTExecutionContext context);
    }
}
