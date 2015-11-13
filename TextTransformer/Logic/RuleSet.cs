using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using TextTransformer.UserControls;
using ZCL.RTScript;

namespace TextTransformer.Logic
{
    //This is an adaptor class used to execute the rules.
    [Serializable]
    public class RuleSet
    {
        private readonly List<RTRuleData> Rules = new List<RTRuleData>();

        public void AddRule(RTRuleData rule)
        {
            Rules.Add(rule);
        }

        public string Run(string source)
        {
            RTRuleSet interpreter = new RTRuleSet(source, Rules.ToArray());
            return interpreter.ExecuteAll();
        }

        public RTRuleData At(int index)
        {
            return Rules[index];
        }

        public int Count
        {
            get
            {
                return Rules.Count;
            }
        }

    }

}
