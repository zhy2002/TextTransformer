using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ZCL.RTScript.DataModel;
using ZCL.RTScript.Logic;

namespace ZCL.RTScript
{
    [Serializable]
    public class RuleOptions
    {
        private MatchOptions _matchOptions;
        private TemplateOptions _templateOptions;
        private MergeOptions _mergeOptions;

        public RuleOptions()
        {
            _matchOptions = new MatchOptions();
            _templateOptions = new TemplateOptions();
            _mergeOptions = new MergeOptions();
        }

        internal MatchOptions MatchOptions
        {
            get
            {
                return _matchOptions;
            }
        }

        internal TemplateOptions TemplateOptions
        {
            get
            {
                return _templateOptions;
            }
        }

        internal MergeOptions MergeOptions
        {
            get
            {
                return _mergeOptions;
            }
        }

        public RegexOptions RegexOptions
        {
            get
            {
                return _matchOptions.RegexOptions;
            }

            set
            {
                _matchOptions.RegexOptions = value;
            }
        }

        public MatchType MatchType
        {
            get
            {
                return _matchOptions.MatchType;
            }

            set
            {
                _matchOptions.MatchType = value;
            }
        }

    }
}
