
namespace ZCL.RTScript.AbstractionLayer
{
    /// <summary>
    /// A matcher parses the source text into a match list, which logically is a jagged array, e.g. string[][].
    /// </summary>
    public interface IRTMatcher
    {
        /// <summary>
        /// This function must be reentrant.
        /// </summary>
        /// <param name="srcText"></param>
        /// <returns></returns>
        IRTMatchList Execute(string srcText);
    }
}
