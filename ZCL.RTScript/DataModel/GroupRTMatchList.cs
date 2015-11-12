
namespace ZCL.RTScript.DataModel
{
    internal class GroupRTMatchList : BaseRTMatchList
    {
        public override void Append(System.Text.RegularExpressions.Match match, int matchIndex)
        {
            var groupMatch = new GroupRTMatch();
            groupMatch.Init(match, matchIndex);
            this._matches.Add(groupMatch); 
        }
    }
}
