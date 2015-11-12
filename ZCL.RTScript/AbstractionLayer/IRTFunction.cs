
using System.Collections.Generic;


namespace ZCL.RTScript.AbstractionLayer
{

    public interface IRTFunction : IRTExpression
    {
        string Name
        {
            get;
        }

        IList<object> Args
        {
            get;
        }

        int SourceStartIndex
        {
            get;
        }

        int SourceEndIndex
        {
            get;
        }

        RTMetadata Metadata
        {
            get;
        }

    }
}
