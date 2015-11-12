using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using RexReplace.GUI.UserControls;

namespace RexReplace.GUI.Logic
{
    [Serializable]
    public class Rule
    {
        public Regex Match
        {
            get;
            set;
        }

        public string Replacement
        {
            get;
            set;
        }

        public SortingOptions SortingOptions
        {
            get;
            set;
        }

        public int SortingGroup
        {
            get;
            set;
        }

        public bool DiscardUnmatchedText
        {
            get;
            set;
        }

        public RegexOptions RuleOptions
        {
            get
            {
                return Match.Options;
            }
        }

        public Rule(string exprmatch, string replacement, RegexOptions options)
        {
            this.Match = new Regex(exprmatch, options);
            this.Replacement = replacement;
        }
    }
}
