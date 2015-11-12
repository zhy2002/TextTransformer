

namespace ZCL.RTScript.AbstractionLayer
{
    public interface IRTTemplate
    {
        string Execute(IRTMatch data, int totalMatchCount);
    }
}
