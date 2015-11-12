

using ZCL.RTScript.AbstractionLayer;

namespace ZCL.RTScript.DataModel
{
    internal abstract class BaseRTMatch : IRTMatch
    {
        protected string[] _items = null;
        protected int[] _srcIndices = null;
        protected int _matchIndex = -1; //the index of the match in all matches

        public string GetItem(int index)
        {
            return _items[index];
        }

        public int GetSourceIndex(int index)
        {
            return _srcIndices[index];
        }

        public int Count
        {
            get { 
                return _items.Length;
            }
        }

        public abstract int GetGroupIndex(int index);

        public int MatchIndex
        {
            get
            {
                return _matchIndex;
            }
        }



        public abstract int SourceStartIndex
        {
            get;
        }

        public abstract int SourceEndIndex
        {
            get;
        }
    }
}
