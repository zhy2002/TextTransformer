using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using ZCL.RTScript.AbstractionLayer;
using ZCL.RTScript.DataModel;
using ZCL.RTScript.Logic.Execution;
using ZCL.RTScript.Logic.Expression;
using ZCL.RTScript.Logic.Metadata;

namespace ZCL.RTScript.Logic
{
    public class RTTemplate : IRTTemplate
    {
        private string _templateExpr;
        private TemplateOptions _options;
        private IRTInterpreter _interpreter;
        private RTParsingException _parsingException;

        public RTTemplate(string templateExpr, TemplateOptions options)
        {
            Contract.Requires(templateExpr != null);
            Contract.Requires(options != null);

            _templateExpr = templateExpr;
            _options = options;
        }

        private void Init()
        {
            if (_options != null)
            {
                RTParsingContext context = new RTParsingContext(_factory);
                var exprList = new RTExpressionList();
                exprList.Parse(_templateExpr, 0, context); //parse should not throw an exception
                _interpreter = exprList;

                _options = null;
                _templateExpr = null;

                if (!_interpreter.CanExecute)
                {
                    _parsingException = new RTParsingException(context.Errors, ErrorMessages.Template_Parsing_Error);
                }
            }
            if (_parsingException != null) throw _parsingException;
        }

        public string Execute(IRTMatch data) {
            return Execute(data, data == null ? 0 : 1);
        }

        public string Execute(IRTMatch data, int totalMatchCount)
        {
            Init();

            RTExecutionContext context = new RTTemplateContext(_templateExpr, data, _factory, totalMatchCount);
            var result = _interpreter.Execute(context);
            if (context.HasError)
            {
                throw new RTExecutionException(context.Errors, ErrorMessages.Template_Execution_Error);
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

        private static TemplateMetadataFactory _factory = new TemplateMetadataFactory(); //todo need to make this extensible, i.e. load metadata from a conventional location
    }
}
