using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using ZCL.RTScript.Logic.Expression;

namespace ZCL.RTScript
{
    public class RTParsingException : ApplicationException, ISerializable
    {
        public RTParsingException(IList<RTParsingError> errors, string message)
            : base(message)
        {
            this.Errors = errors;
        }

        public IList<RTParsingError> Errors
        {
            get;
            private set;
        }
    }
}
