using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using ZCL.RTScript.Logic.Execution;

namespace ZCL.RTScript
{
    public class RTExecutionException : ApplicationException, ISerializable
    {
        public RTExecutionException(IList<RTExecutionError> errors, string message)
            : base(message)
        {
            this.Errors = errors;
        }

        public IList<RTExecutionError> Errors
        {
            get;
            private set;
        }
    }
}
