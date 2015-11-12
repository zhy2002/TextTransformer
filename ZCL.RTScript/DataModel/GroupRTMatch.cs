
using System.Text.RegularExpressions;
using ZCL.RTScript.AbstractionLayer;

namespace ZCL.RTScript.DataModel
{
    internal class GroupRTMatch : BaseRTMatch, IRTMatch
    {
        public void Init(Match match, int matchIndex)
        {
            this._matchIndex = matchIndex;
            this._items = new string[match.Groups.Count];
            this._srcIndices = new int[_items.Length];
            for (int i = 0; i < this._items.Length; i++)
            {
                this._items[i] = match.Groups[i].Value;
                this._srcIndices[i] = match.Groups[i].Index;
            }
        }

        public override int GetGroupIndex(int index)
        {
            return index;
        }

        public override int SourceStartIndex
        {
            get { return GetSourceIndex(0); }
        }

        public override int SourceEndIndex
        {
            get { return this.SourceStartIndex + GetItem(0).Length - 1; }
        }
    }
}
