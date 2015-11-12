using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ZCL.RTScript.AbstractionLayer;
using ZCL.RTScript.DataModel;

namespace ZCL.RTScript.Logic
{
    /// <summary>
    /// This is how a Rule transforms the source text:
    /// 1. Load Rule options
    /// 2. Use Matcher to capture matched parts of the source text into a jagged array (called a table of rows and a row of items although not exactly a table because of the jaggedness); some context information about the capture of an item and a row is kept.
    /// 3. Use Template to transform a row into a string and pass a list of string to Merger. Merger still has access to the row where the template used to produce the result string.
    /// 4. Use Merger to combine the strings into a single string, which is then returned as the result. Merger has available to it the input and output of all Template transformation.
    /// </summary>
    internal class RTRule
    {
        private RTRuleData _data;
        private IRTMatcher _matcher;
        private IRTTemplate _template;
        private IRTMerger _merger;

        public RTRule(RTRuleData ruleData)
        {
            Contract.Requires(ruleData != null);

            this._data = ruleData;
        }

        /// <summary>
        /// Initialisation is delayed to actual execution
        /// </summary>
        private void Init()
        {
            if (this._data != null)
            {
                _matcher = new RTMatcher(this._data.MatchExpression, _data.RuleOptions.MatchOptions);
                _template = new RTTemplate(_data.TemplateExpression, _data.RuleOptions.TemplateOptions);
                _merger = new RTMerger(this._data.MergeExpression, this._data.RuleOptions.MergeOptions);
                this._data = null;
            }
        }

        public string Execute(string srcText)
        {
            Init(); //lazy initialisation

            var matcherOutput = _matcher.Execute(srcText);
            IRTEntry[] mergerInput = new RTEntry[matcherOutput.Count];//each matched row is mapped to an entry; mergerInput.Length = matcherOutput.Count
            for (int i = 0; i < matcherOutput.Count; i++)
            {
                string replacement = _template.Execute(matcherOutput[i], matcherOutput.Count);
                mergerInput[i] = new RTEntry() { DataSource = matcherOutput[i], Result = replacement, EntryIndex = i };
            }
            return this._merger.Execute(srcText, mergerInput);
        }

    }
}
