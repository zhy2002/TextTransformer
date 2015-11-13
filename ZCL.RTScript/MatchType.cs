using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace ZCL.RTScript
{
    public enum MatchType
    {
        [Description("Group Per Item")]
        Group, //default
        [Description("Capture Per Item")]
        Capture
    }
}
