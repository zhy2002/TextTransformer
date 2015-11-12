using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RegTransf.Core
{
    public interface IMatcher
    {
        /// <summary>
        /// Return null when run out of match.
        /// </summary>
        /// <returns></returns>
        IMatch GetNext();
    }
}
