using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RegTransf.Core
{
    public interface IMatch
    {
        string SourceIndex
        {
            get;
        }

        string Item
        {
            get;
        }

        /// <summary>
        /// Item Count
        /// </summary>
        string Count
        {
            get;
        }
    }

}
