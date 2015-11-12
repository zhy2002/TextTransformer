

namespace ZCL.RTScript.AbstractionLayer
{
    public interface IRTMerger
    {
        string Execute(string srcText, IRTEntry[] source);
    }
}
