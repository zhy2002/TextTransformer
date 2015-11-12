using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZCL.RTScript.AbstractionLayer;
using System.Text.RegularExpressions;
using ZCL.RTScript.DataModel;

namespace ZCL.RTScript.Logic
{
    /// <summary>
    /// The default matcher, which parses the source string using regular expression.
    /// </summary>
    public class RTMatcher : IRTMatcher
    {
        private MatchOptions _options;

        public RTMatcher(string matchExpr, MatchOptions options)
        {
            _options = options;
            Regex = new Regex(matchExpr, _options.RegexOptions);
        }

        public Regex Regex
        {
            get;
            private set;
        }

        public IRTMatchList Execute(string srcText)
        {
            //dependency
            BaseRTMatchList result = _options.MatchType == MatchType.Capture ? new CaptureRTMatchList() as BaseRTMatchList : new GroupRTMatchList() as BaseRTMatchList;

            Match match = Regex.Match(srcText);
            int index = 0;
            while (match.Success)
            {
                result.Append(match, index++);
                match = match.NextMatch();
            }

            return result;
        }


       
    }
}
