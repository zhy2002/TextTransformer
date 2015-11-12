using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZCL.RTScript.Logic.Execution;

namespace ZCL.RTScript.Logic.Expression
{
    public static class RTSysSymbols
    {
        public const string APPLY = "=>";
        public const string DEF = "::";
        public const string SET = ":=";
        public const string FOR = "for";
        public const string IF = "if";

        public const string EQUAL = "=";
        public const string NOT_EQUAL = "!=";
        public const string GREATER = ">";
        public const string LESS = "<";
        public const string GREATER_EQUAL = ">=";
        public const string LESS_EQUAL = "<=";
        public const string RESIDUE_IS = "residueIs";

        public const string SCOPE = "scope";

        public const string GET = "$";
        public const string ITEM_COUNT = "itemCount";
        public const string MERGE = "merge";

        public const string ADD = "+";
        public const string MUL = "*";

        public const string MERGE_INPUT = "$mergeInput";
        public const string SOURCE_TEXT = "$srcText";
        public const string MATCH_INPUT = "$matchInput";
        public const string DEFINING_SYMBOL = "$definingSymbol";




    }
}
