
using ZCL.RTScript.AbstractionLayer;

namespace ZCL.RTScript.DataModel
{
    internal class RTEntry : IRTEntry
    {
        public IRTMatch DataSource
        {
            get;
            set;
        }

        public string Result
        {
            get;
            set;
        }

        public int EntryIndex
        {
            get;
            set;
        }
       
    }
}
