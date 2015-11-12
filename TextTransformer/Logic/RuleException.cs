using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace RexReplace.GUI.Logic
{
    [Serializable]
    public class RuleException : ApplicationException, ISerializable
    {
        private static readonly string MSG_SUFFIX_TEMPLATE = " at line {LineNumber} column {ColumnNumber} in rule {RuleId}.";

        protected RuleException(SerializationInfo info, StreamingContext context) :
            base(info, context)
        {
            _message = info.GetString("_message");
            _sourceCode = info.GetString("_sourceCode");
            _opName = info.GetString("_opName");
            _minArgNumber = info.GetInt32("_minArgNumber");
            _maxArgNumber = info.GetInt32("_maxArgNumber");
            _actualArgNumber = info.GetInt32("_actualArgNumber");
            _ruleId = info.GetInt32("_ruleId");
            _sourceIndex = info.GetInt32("_sourceIndex");
            _lineNumber = info.GetInt32("_lineNumber");
            _columnNumber = info.GetInt32("_columnNumber");
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("_message", _message);
            info.AddValue("_sourceCode", _sourceCode);
            info.AddValue("_opName", _opName);
            info.AddValue("_minArgNumber", _minArgNumber);
            info.AddValue("_maxArgNumber", _maxArgNumber);
            info.AddValue("_actualArgNumber", _actualArgNumber);
            info.AddValue("_ruleId", _ruleId);
            info.AddValue("_sourceIndex", _sourceIndex);
            info.AddValue("_lineNumber", _lineNumber);
            info.AddValue("_columnNumber", _columnNumber);

            base.GetObjectData(info, context);
        }

        /// <summary>
        /// Each property can only be set once!
        /// </summary>
        /// <param name="errorMessage"></param>
        /// <param name="expr"></param>
        public RuleException(string errorMessage, Exception innerEx = null)
            : base(null, innerEx)
        {
            _message = errorMessage;
        }

        private string _sourceCode;

        public string SourceCode
        {
            get { return _sourceCode; }
            set
            {
                if (_sourceCode != null || value == null) throw new InvalidOperationException();
                _sourceCode = value;
            }
        }

        private string _opName;
        public string OpName
        {
            get { return _opName; }
            set
            {
                if (_opName != null || value == null) throw new InvalidOperationException();
                _opName = value;
                _message = _message.Replace("{OpName}", _opName);
            }
        }

        private int _minArgNumber = -1;
        public int MinArgNumber
        {
            get { return _minArgNumber; }
            set
            {
                if (_minArgNumber != -1 || value < 0) throw new InvalidOperationException();
                _minArgNumber = value;
                _message = _message.Replace("{MinArgNumber}", _minArgNumber.ToString());
            }
        }

        private int _maxArgNumber = -1;
        public int MaxArgNumber
        {
            get { return _maxArgNumber; }
            set
            {
                if (_maxArgNumber != -1 || value < 0) throw new InvalidOperationException();
                _maxArgNumber = value;
                _message = _message.Replace("{MaxArgNumber}", _maxArgNumber.ToString());
            }
        }

        private int _actualArgNumber = -1;
        public int ActualArgNumber
        {
            get { return _actualArgNumber; }
            set
            {
                if (_actualArgNumber != -1 || value < 0) throw new InvalidOperationException();
                _actualArgNumber = value;
                _message = _message.Replace("{ActualArgNumber}", _actualArgNumber.ToString());
            }
        }

        private int _ruleId = -1;
        public int RuleId
        {
            get { return _ruleId; }
            set
            {
                if (_ruleId != -1 || value < 0) throw new InvalidOperationException();
                _ruleId = value;
                if (_sourceCode != null && _sourceIndex == -1)
                {
                    _message += MSG_SUFFIX_TEMPLATE;
                }
                _message = _message.Replace("{RuleId}", _ruleId.ToString());
            }
        }

        private int _sourceIndex = -1;
        public int SourceIndex
        {
            get { return _sourceIndex; }
            set
            {
                //must be assigned after sourcecode property
                if (_sourceCode == null || _sourceIndex != -1 || value < 0) throw new InvalidOperationException();

                _sourceIndex = value;
                _message = _message.Replace("{SourceIndex}", _sourceIndex.ToString());

                if (_sourceCode != null && _ruleId == -1)
                {
                    _message += MSG_SUFFIX_TEMPLATE;
                }

                int line = 1, col = 0;
                for (int i = 0; i <= _sourceIndex; i++)
                {

                    if (_sourceCode[i] == '\n')
                    {
                        line++;
                        col = 0;
                    }
                    else if (char.IsControl(_sourceCode[i])) continue;
                    else
                    {
                        col++;
                    }
                }

                LineNumber = line;
                ColumnNumber = col;

            }
        }

        private int _lineNumber = -1;
        public int LineNumber
        {
            get { return _lineNumber; }
            private set
            {
                _lineNumber = value;
                _message = _message.Replace("{LineNumber}", _lineNumber.ToString());
            }
        }

        private int _columnNumber = -1;
        public int ColumnNumber
        {
            get { return _columnNumber; }
            private set
            {
                _columnNumber = value;
                _message = _message.Replace("{ColumnNumber}", _columnNumber.ToString());
            }
        }

        private string _message;
        public override string Message
        {
            get
            {
                return _message;
            }
        }
    }
}
