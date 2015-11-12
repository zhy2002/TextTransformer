
using ZCL.RTScript.AbstractionLayer;
using ZCL.RTScript.Logic.Expression;

namespace ZCL.RTScript.Logic.Execution
{
    public class RTTemplateContext : RTExecutionContext
    {
        private readonly IRTMatch _matchInput;
        private readonly int _totalMatchCount;

        public RTTemplateContext(string srcText, IRTMatch data, IRTMetadataFactory factory, int totalMatchCount)
            : base(factory)
        {
            this._srcText = srcText;
            this._matchInput = data;
            this._totalMatchCount = totalMatchCount;
        }

        public IRTMatch RTMatch {
            get {
                return _matchInput;
            }
        }

        public int RTMatchCount {
            get {
                return _totalMatchCount;
            }
        }


        protected override void InitRootScope()
        {
            base.InitRootScope();

            CurrentScope.SetValueLocal(RTSysSymbols.MATCH_INPUT, _matchInput);
        }
    }
}
