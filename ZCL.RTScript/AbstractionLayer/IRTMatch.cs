

namespace ZCL.RTScript.AbstractionLayer
{
    /// <summary>
    /// A match is a list of items.
    /// </summary>
    public interface IRTMatch
    {
        /// <summary>
        /// Get a matched text in the list.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        string GetItem(int index);

        int Count
        {
            get;
        }

        /// <summary>
        /// Where in the source text the item at <paramref name="index"/> starts.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        int GetSourceIndex(int index);

        /// <summary>
        /// where the Match starts in the source text.
        /// </summary>
        int SourceStartIndex
        {
            get;
        }

        /// <summary>
        /// where the Match ends in the source text.
        /// </summary>
        int SourceEndIndex
        {
            get;
        }

        /// <summary>
        /// The capture group that the item belongs to.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        int GetGroupIndex(int index);

        /// <summary>
        /// The index of the original Regex Match that captures this list of items.
        /// </summary>
        int MatchIndex
        {
            get;
        }

    }
}
