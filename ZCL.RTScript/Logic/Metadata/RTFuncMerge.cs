using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using ZCL.RTScript.AbstractionLayer;
using ZCL.RTScript.Logic.Execution;
using ZCL.RTScript.Logic.Expression;

namespace ZCL.RTScript.Logic.Metadata
{
    class RTFuncMerge : RTMetadata
    {

        private IList<string> ExtractUnmatched(string srcText, IRTEntry[] source)
        {
            IList<string> unmatched = new List<string>();
            int srcNextIndex = 0;
            for (int i = 0; i < source.Length; i++)
            {
                if (source[i].DataSource.SourceStartIndex > srcNextIndex)
                {
                    unmatched.Add(srcText.Substring(srcNextIndex, source[i].DataSource.SourceStartIndex - srcNextIndex));
                }
                unmatched.Add(null);
                srcNextIndex = source[i].DataSource.SourceEndIndex + 1;
            }
            if (srcNextIndex < srcText.Length)
            {
                unmatched.Add(srcText.Substring(srcNextIndex));
            }
            return unmatched;
        }

        public override string FunctionName
        {
            get { return RTSysSymbols.MERGE; }
        }

        public override object Execute(RTExecutionContext context)
        {
            IRTEntry[] source = (new RTVariable(RTSysSymbols.MERGE_INPUT)).Execute(context) as IRTEntry[];
            string srcText = (new RTVariable(RTSysSymbols.SOURCE_TEXT)).Execute(context) as string;
            IList<string> unmatched = ExtractUnmatched(srcText, source);
            string[] processed = source.Select(x => x.Result).ToArray();
            Debug.Assert(unmatched.Count >= processed.Length);

            StringBuilder sb = new StringBuilder();
            int j = 0;
            for (int i = 0; i < unmatched.Count; i++)
            {
                if (unmatched[i] != null)
                {
                    sb.Append(unmatched[i]);
                }
                else
                {
                    if (j < processed.Length)
                    {
                        sb.Append(processed[j++]);
                    }
                }
            }
            return sb.ToString();
        }
    }
}
