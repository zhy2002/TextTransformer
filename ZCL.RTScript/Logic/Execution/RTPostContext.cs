
using ZCL.RTScript.AbstractionLayer;
using ZCL.RTScript.Logic.Expression;


namespace ZCL.RTScript.Logic.Execution
{
    internal class RTPostContext : RTExecutionContext
    {
        private IRTEntry[] _mergeInput;


        protected override void InitRootScope()
        {
            base.InitRootScope();

            CurrentScope.SetValueLocal(RTSysSymbols.MERGE_INPUT, _mergeInput);
            CurrentScope.SetValueLocal(RTSysSymbols.SOURCE_TEXT, _srcText);
        }

        public RTPostContext(string srcText, IRTEntry[] mergeInput, IRTMetadataFactory factory)
            : base(factory)
        {
            this._srcText = srcText;
            this._mergeInput = mergeInput;
        }



    }
}
