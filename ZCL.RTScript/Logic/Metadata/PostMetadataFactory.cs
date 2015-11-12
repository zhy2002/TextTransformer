using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZCL.RTScript.Logic.Execution;

namespace ZCL.RTScript.Logic.Metadata
{
    public class PostMetadataFactory : BaseMetadataFactory, IRTMetadataFactory
    {
        protected override void Init()
        {
            base.Init();
            this.AddMetadata(new RTFuncMerge());
            //todo merge get, unmatchedbetween
        }
    }
}
