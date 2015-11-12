using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using ZCL.RTScript.AbstractionLayer;
using ZCL.RTScript.Logic.Execution;
using ZCL.RTScript.Logic.Expression;
using ZCL.RTScript.Logic.Metadata;

namespace ZCL.RTScript.Logic
{
    public class RTMerger : IRTMerger
    {
        private string _mergeExpr;
        private MergeOptions _mergeOptions;
        private RTExpressionList _interpreter;
        private RTParsingException _parsingException;

        public RTMerger(string mergeExpr, MergeOptions options)
        {
            _mergeExpr = mergeExpr;
            _mergeOptions = options;
        }

        private void Init()
        {
            if (_mergeOptions != null)
            {
                RTParsingContext context = new RTParsingContext(_factory);
                var exprList = new RTExpressionList();
                exprList.Parse(_mergeExpr, 0, context);
                _interpreter = exprList;

                _mergeOptions = null; //not needed anymore
                _mergeExpr = null;

                if (!_interpreter.CanExecute)
                {
                    _parsingException = new RTParsingException(context.Errors, ErrorMessages.Post_Parsing_Error);
                }
            }
            if (_parsingException != null) throw _parsingException;
        }


        public string Execute(string srcText, IRTEntry[] source)
        {
            Init();

            RTExecutionContext context = new RTPostContext(srcText, source, _factory);
            var result = _interpreter.Execute(context);
            if (context.HasError)
            {
                throw new RTExecutionException(context.Errors, ErrorMessages.Post_Execution_Error);
            }

            if (result == null)
            {
                return null;
            }
            else
            {
                return result.ToString();
            }
        }

        private static PostMetadataFactory _factory = new PostMetadataFactory(); //todo need to make this extensible, i.e. load metadata from a conventional location

    }
}
