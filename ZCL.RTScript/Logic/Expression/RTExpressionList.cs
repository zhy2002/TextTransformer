using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZCL.RTScript.AbstractionLayer;
using ZCL.RTScript.Logic.Execution;

namespace ZCL.RTScript.Logic.Expression
{
    public class RTExpressionList : RTExpression, IRTInterpreter
    {
        private IList<RTExpression> _items = new List<RTExpression>();

        protected override void ParseInternal(string srcText, RTParsingContext context)
        {
            int enteringDepth = context.MarkerDepth;
            int index = this.SourceStartIndex;
            if (enteringDepth > 0)
            {
                context.MarkerDepth++;
                index += 2;
            }

            while (index < srcText.Length)
            {
                RTExpression expr;
                MarkerType markerType = PeekMarker(srcText, index);

                if (markerType == MarkerType.StartMarker)
                {
                    expr = new RTFunctionList();
                }
                else if (markerType == MarkerType.EndMarker)
                {
                    if (context.MarkerDepth > 0)
                    {
                        context.MarkerDepth--;
                        context.SourceEndIndex = index + 1;
                        break;
                    }
                    else
                    {
                        expr = new RTLiteral();
                    }
                }
                else
                {
                    expr = new RTLiteral();
                }

                expr.Parse(srcText, index, context);
                _items.Add(expr);
                index = context.SourceEndIndex + 1;

            }

            if (enteringDepth != context.MarkerDepth)
            {
                context.MarkerDepth = enteringDepth; //restore from error
                context.SourceEndIndex = index - 1;
                context.Errors.Add(new RTParsingError(this.SourceStartIndex, context.SourceEndIndex, RTErrorCode.EndMarkerMissing, ErrorMessages.Expression_List_End_Marker_Missing));
            }
        }

        public override object Execute(RTExecutionContext context)
        {
            StringBuilder sb = new StringBuilder();

            foreach (var expr in _items)
            {
                var retval = expr.Execute(context);
                if (!context.HasError)
                {
                    sb.Append(retval); //literal and function list return string
                }
            }

            if (!context.HasError) return sb.ToString();
            else return null;
        }

    }
}
