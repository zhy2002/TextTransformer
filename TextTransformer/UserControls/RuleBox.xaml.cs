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

using RexReplace.GUI.Logic;
using System.ComponentModel;

namespace RexReplace.GUI.UserControls
{
    /// <summary>
    /// Properties for RuleBox.xaml which read from or write the relevant UI elements.
    /// </summary>
    public partial class RuleBox : UserControl
    {
        public RuleBox()
        {
            InitializeComponent();
            RegOptions = RegexOptions.None;
            this.SortingOption = SortingOptions.None;
            this.DiscardUnmatchText = false;
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
                stext.Text = "Substitute " + (index + 1);
                mtext.Text = "Match " + (index + 1);
                sortingtext.Text = "Sorting Group " + (index + 1);
            }
        }

        /// <summary>
        /// Get or set the replace rule represented by this control.
        /// </summary>
        public Rule ReplaceRule
        {
            get
            {
                if (mbox.Text == string.Empty)
                {
                    throw new RuleException("Match expression is invalid.");
                }

                int sortingGroup;
                if (!int.TryParse(sortingbox.Text, out sortingGroup) || sortingGroup < 0)
                {
                    throw new RuleException("Sorting expression is invalid.");
                }
                return new Rule(mbox.Text, sbox.Text, RegOptions) { SortingOptions = this.SortingOption, SortingGroup = sortingGroup, DiscardUnmatchedText = this.DiscardUnmatchText };
            }

            set
            {
                if (value == null)
                {
                    sbox.Text = string.Empty;
                    mbox.Text = string.Empty;
                    sortingbox.Text = "0";
                    RegOptions = RegexOptions.None;
                    this.DiscardUnmatchText = false;
                    this.SortingOption = SortingOptions.None;
                    return;
                }
                sbox.Text = value.Replacement;
                mbox.Text = value.Match.ToString();
                sortingbox.Text = value.SortingGroup.ToString();
                RegOptions = value.RuleOptions;
                this.DiscardUnmatchText = value.DiscardUnmatchedText;
                this.SortingOption = value.SortingOptions;
            }
        }

        public RegexOptions RegOptions
        {
            get;
            set;
        }

        public bool DiscardUnmatchText
        {
            get;
            set;
        }

        private SortingOptions _sorOption;

        public SortingOptions SortingOption
        {
            get
            {
                return _sorOption;
            }

            set
            {
                _sorOption = value;
                if (_sorOption == SortingOptions.None)
                {
                    sortingbox.IsEnabled = false;
                    sortingtext.ToolTip = "Enable sorting by selecting an option in the context menu.";
                }
                else
                {
                    sortingbox.IsEnabled = true;
                    sortingtext.ToolTip = null;
                }
            }
        }

    }

}
