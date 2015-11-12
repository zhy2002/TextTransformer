

namespace ZCL.RTScript.AbstractionLayer
{
    /// <summary>
    /// Output of the Match step.
    /// </summary>
    public interface IRTMatchList
    {
        IRTMatch this[int index]
        {
            get;
        }

        int Count
        {
            get;
        }
    }
}
