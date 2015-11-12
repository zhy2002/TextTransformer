using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace RexReplace.GUI.UserControls
{
    public enum SortingOptions
    {
        [Description("Original Order")]
        None, //no sorting
        [Description("Reverse")]
        Reverse, //reverse the original order
        [Description("String Ascending")]
        Asc, //as strings
        [Description("String Descending")]
        Desc,
        [Description("Number Ascending")]
        NumberAsc,//assuming the matched text has a number prefix
        [Description("Number Descending")]
        NumberDesc,
        [Description("Datetime Ascending")]
        DatetimeAsc, //matched text should be able to parse as datetime
        [Description("Datetime Descending")]
        DatetimeDesc
    }
}
