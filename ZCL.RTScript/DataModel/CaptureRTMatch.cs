
using ZCL.RTScript.AbstractionLayer;

namespace ZCL.RTScript.DataModel
{
    internal class CaptureRTMatch : BaseRTMatch, IRTMatch
    {
        private int _groupIndex;

        public void Init(System.Text.RegularExpressions.Match match, int matchIndex, int groupIndex)
        {
            this._matchIndex = matchIndex;
            this._groupIndex = groupIndex;
            this._items = new string[match.Groups[_groupIndex].Captures.Count];
            this._srcIndices = new int[this._items.Length];
            for (int i = 0; i < this._items.Length; i++)
            {
                this._items[i] = match.Groups[_groupIndex].Captures[i].Value;
                this._srcIndices[i] = match.Groups[_groupIndex].Captures[i].Index;
            }

        }

        public override int GetGroupIndex(int index)
        {
            return _groupIndex;
        }


        public override int SourceStartIndex
        {
            get { return GetSourceIndex(0); }
        }

        public override int SourceEndIndex
        {
            get
            {
                int lastIndex = this.Count - 1;
                return GetSourceIndex(lastIndex) + GetItem(lastIndex).Length - 1;
            }
        }

    }
}
