using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZCL.RTScript
{
    public enum RTErrorCode
    {
        EndMarkerMissing,
        EndBraketMissing,
        InvalidToken,
        InvalidNumber,
        InvalidIdentifier,
        InvalidFunctionName,
        OpenBracketMissing,
        Metadata_Not_Enough_Args,
        Metadata_Too_Many_Args,
        Metadata_Invalid_Arg,
        SysFuncError,
        LibFuncError
    }
}
