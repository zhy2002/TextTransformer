using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using ZCL.RTScript;

namespace ZCL.RTScript
{
    /// <summary>
    /// The instance of this class represent the state of a rule.
    /// </summary>
    [Serializable]
    public class RTRuleData
    {
        public RTRuleData(string matchExpr, string templateExpr, string mergeExpr, RuleOptions options)
        {
            this.MatchExpression = matchExpr;
            this.TemplateExpression = templateExpr;
            this.MergeExpression = mergeExpr;
            this.RuleOptions = options;
        }

        public string MatchExpression
        {
            get;
            private set;
        }

        public string TemplateExpression
        {
            get;
            private set;
        }

        public string MergeExpression
        {
            get;
            private set;
        }

        public RuleOptions RuleOptions
        {
            get;
            private set;
        }
    }
}
