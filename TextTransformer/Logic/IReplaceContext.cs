using System;
namespace RexReplace.GUI.Logic
{
    interface IReplaceContext
    {
        int MatchCount { get; }
        int MatchId { get; }
        System.Text.RegularExpressions.GroupCollection MatchValues { get; }
    }
}
