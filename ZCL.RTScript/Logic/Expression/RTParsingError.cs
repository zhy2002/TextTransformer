using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZCL.RTScript.Logic.Expression
{
    /// <summary>
    /// a parsing error entry
    /// </summary>
    public class RTParsingError
    {
        public RTParsingError(int startIndex, int endIndex, RTErrorCode errorCode, string errorMessage)
        {
            this._startIndex = startIndex;
            this._endIndex = endIndex;
            this._errorCode = errorCode;
            this._errorMessage = errorMessage;
        }

        private int _startIndex;

        public int StartIndex
        {
            get
            {
                return _startIndex;
            }
        }

        private int _endIndex;

        public int EndIndex
        {
            get
            {
                return _endIndex;
            }
        }

        private RTErrorCode _errorCode;

        public RTErrorCode ErrorCode
        {
            get
            {
                return _errorCode;
            }

        }

        private string _errorMessage;

        public string ErrorMessage
        {
            get
            {
                return _errorMessage;
            }
        }

    }
}
