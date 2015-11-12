

using ZCL.RTScript.Logic.Execution;

namespace ZCL.RTScript.AbstractionLayer
{
    public interface IRTInterpreter
    {
        bool CanExecute
        {
            get;
        }

        object Execute(RTExecutionContext context);
    }
}
