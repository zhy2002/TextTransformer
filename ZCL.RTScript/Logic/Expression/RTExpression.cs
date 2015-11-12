
using System.Diagnostics.Contracts;
using ZCL.RTScript.AbstractionLayer;
using ZCL.RTScript.Logic.Execution;

namespace ZCL.RTScript.Logic.Expression
{
    public abstract class RTExpression : IRTExpression
    {

        public int SourceStartIndex
        {
            get;
            private set;
        }

        public int SourceEndIndex
        {
            get;
            private set;
        }

        /// <summary>
        /// True if there is no parsing errors.
        /// </summary>
        public bool CanExecute
        {
            get;
            private set;
        }

        public void Parse(string srcText, RTParsingContext context)
        {
            Parse(srcText, 0, context);
        }

        public void Parse(string srcText, int srcStartIndex, RTParsingContext context)
        {
            Contract.Requires(srcStartIndex >= 0);
            Contract.Requires(srcStartIndex < srcText.Length);
            Contract.Requires(context != null);

            this.SourceStartIndex = srcStartIndex;
            var existingErrors = context.Errors.Count;
            ParseInternal(srcText, context);
            if (context.Errors.Count == existingErrors)
                this.CanExecute = true;
            this.SourceEndIndex = context.SourceEndIndex;
        }

        /// <summary>
        /// Need to return source end index with parsing context.
        /// Source start index is passed in through parsing context.
        /// </summary>
        /// <param name="srcText"></param>
        /// <param name="context"></param>
        protected abstract void ParseInternal(string srcText, RTParsingContext context);

        public abstract object Execute(RTExecutionContext context);

        protected static MarkerType PeekMarker(string srcText, int startIndex)
        {
            if (startIndex >= srcText.Length - 1) return MarkerType.None;
            if (srcText[startIndex] == EXPR_MARKER)
            {
                if (srcText[startIndex + 1] == EXPR_START) return MarkerType.StartMarker;
                else if (srcText[startIndex + 1] == EXPR_END) return MarkerType.EndMarker;
            }
            return MarkerType.None;
        }

        protected enum MarkerType
        {
            None,
            StartMarker,
            EndMarker
        }

        protected const char EXPR_MARKER = '@';
        protected const char EXPR_START = '{';
        protected const char EXPR_END = '}';
        protected const char EXPR_FUNC_OPEN = '(';
        protected const char EXPR_FUNC_CLOSE = ')';
    }

}


