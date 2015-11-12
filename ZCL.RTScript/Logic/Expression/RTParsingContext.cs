using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZCL.RTScript.Logic.Execution;

namespace ZCL.RTScript.Logic.Expression
{
    public class RTParsingContext
    {
        public RTParsingContext(IRTMetadataFactory metaFactory)
        {
            this.MetadataFactory = metaFactory;
        }

        /// <summary>
        /// returns if the parsing process is ok
        /// </summary>
        public bool HasError
        {
            get
            {
                return _errors.Count == 0;
            }
        }

        //returns the end index if is Valid
        public int SourceEndIndex
        {
            get;
            set;
        }

        //input tells the index is inside of a marker
        public int MarkerDepth
        {
            get;
            set;
        }

        private IList<RTParsingError> _errors = new List<RTParsingError>();

        public IList<RTParsingError> Errors
        {
            get
            {
                return _errors;
            }
        }

        public IRTMetadataFactory MetadataFactory
        {
            get;
            private set;
        }

    }
}
