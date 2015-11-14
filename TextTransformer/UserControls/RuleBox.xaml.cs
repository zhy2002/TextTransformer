using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using TextTransformer.Logic;
using System.ComponentModel;
using ZCL.RTScript;
using ZCL.RTScript.Logic.Expression.Literal;

namespace TextTransformer.UserControls
{
    /// <summary>
    /// Properties for RuleBox.xaml which read from or write the relevant UI elements.
    /// </summary>
    public partial class RuleBox : UserControl
    {
        public RuleBox()
        {
            InitializeComponent();

            RuleData = null;
        }

        private int index = 0;

        public int Index
        {
            get
            {
                return index;
            }

            set
            {
                index = value;
                matchTextHeader.Text = "Match " + (index + 1);
                tabItemReplace.Header = "Replace " + (index + 1);
                tabItemMerge.Header = "Merge " + (index + 1);
            }
        }

        public RegexOptions RegOptions
        {
            get;
            set;
        }

        public MatchType MatchType
        {
            get;
            set;
        }

        /// <summary>
        /// Get or set the replace rule represented by this control.
        /// </summary>
        public RTRuleData RuleData
        {
            get
            {
                if (matchText.Text == string.Empty)
                {
                    throw new ReplaceException("Match expression cannot be empty.");
                }

                RuleOptions ruleOptions = new RuleOptions();
                ruleOptions.MatchType = this.MatchType;
                ruleOptions.RegexOptions = this.RegOptions;
                RTRuleData ruleData = new RTRuleData(matchText.Text, replaceText.Text, mergeText.Text, ruleOptions);
                return ruleData;
            }

            set
            {
                String matchExpression = string.Empty;
                String replaceExpression = "[{0}]";
                String mergeExpression = "@{(merge)@}";
                RegexOptions regOptions = RegexOptions.None;
                MatchType matchType = MatchType.Group;

                if (value != null) {
                    matchExpression = value.MatchExpression;
                    replaceExpression = value.TemplateExpression;
                    mergeExpression = value.MergeExpression;
                    regOptions = value.RuleOptions.RegexOptions;
                    matchType = value.RuleOptions.MatchType;
                }

                matchText.Text = matchExpression;
                replaceText.Text = replaceExpression;
                mergeText.Text = mergeExpression;
                this.MatchType = matchType;
                this.RegOptions = regOptions;
            }
        }



    }

}
