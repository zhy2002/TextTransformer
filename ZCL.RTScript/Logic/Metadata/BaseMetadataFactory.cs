using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZCL.RTScript.Logic.Metadata
{
    public class BaseMetadataFactory : IRTMetadataFactory
    {
        private IDictionary<string, RTMetadata> _repository = new Dictionary<string, RTMetadata>();

        public BaseMetadataFactory()
        {
            Init();
        }

        protected virtual void Init()
        {
            AddMetadata(new RTFuncDef());
            AddMetadata(new RTFuncApply());
            AddMetadata(new RTFuncSet());
            AddMetadata(new RTFuncFor());
            AddMetadata(new RTFuncIf());
            AddMetadata(new RTLibFuncEqual());
            AddMetadata(new RTLibFuncGreater());//todo test
            AddMetadata(new RTLibFuncGreaterEqual());//todo test
            AddMetadata(new RTLibFuncLess());//todo test
            AddMetadata(new RTLibFuncLessEqual());//todo test
            AddMetadata(new RTLibFuncAdd());
            AddMetadata(new RTLibFuncMul());
            AddMetadata(new RTLibFuncNotEqual());//todo test
            AddMetadata(new RTLibFuncResidueIs());//todo test
            AddMetadata(new RTFuncScope());//todo test
        }

        protected virtual void AddMetadata(RTMetadata metadata)
        {
            _repository.Add(metadata.FunctionName, metadata);
        }

        public RTMetadata GetMetadata(string name)
        {
            RTMetadata retval;
            _repository.TryGetValue(name, out retval);
            return retval;
        }
    }
}
