using System;
using ZCL.RTScript.AbstractionLayer;

namespace ZCL.RTScript.Logic.Expression.Literal
{
    interface IReplaceContext
    {
        int MatchCount { get; }
        int MatchId { get; }
        IRTMatch MatchValues { get; }
    }
}
