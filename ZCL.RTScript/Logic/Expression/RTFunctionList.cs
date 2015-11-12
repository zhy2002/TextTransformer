using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using ZCL.RTScript.Logic.Execution;

namespace ZCL.RTScript.Logic.Expression
{
    internal class RTFunctionList : RTExpression
    {
        private IList<RTExpression> _items = new List<RTExpression>();

        protected override void ParseInternal(string srcText, RTParsingContext context)
        {
            Debug.Assert(PeekMarker(srcText, SourceStartIndex) == MarkerType.StartMarker);
            int enteringDepth = context.MarkerDepth;
            context.MarkerDepth++;
            int index = this.SourceStartIndex + 2;

            while (index < srcText.Length)
            {
                RTExpression expr;
                if (Char.IsWhiteSpace(srcText[index]))
                {
                    index++;
                    continue;
                }

                MarkerType markerType = PeekMarker(srcText, index);
                if (markerType == MarkerType.StartMarker)
                {
                    expr = new RTExpressionList();
                }
                else if (markerType == MarkerType.None)
                {
                    expr = new RTFunction();
                }
                else //== EndMarker
                {
                    context.MarkerDepth--;
                    context.SourceEndIndex = index + 1;
                    break;
                }

                expr.Parse(srcText, index, context);
                _items.Add(expr);
                index = context.SourceEndIndex + 1;
            }

            if (enteringDepth != context.MarkerDepth)
            {
                context.MarkerDepth = enteringDepth; //restore from error
                context.SourceEndIndex = index - 1;
                context.Errors.Add(new RTParsingError(this.SourceStartIndex, context.SourceEndIndex, RTErrorCode.EndMarkerMissing, ErrorMessages.Function_List_End_Marker_Missing));
            }

        }

        public override object Execute(RTExecutionContext context)
        {
            StringBuilder sb = new StringBuilder();
            context.PushScope(new RTScope(null));
            foreach (var func in _items)
            {
                var result = func.Execute(context);
                if (!context.HasError && result != null && !(result is RTVoid))
                {
                    sb.Append(result);
                }
            }
            context.PopScope();
            if (!context.HasError) return sb.ToString();
            else return null;
        }



    }
}
